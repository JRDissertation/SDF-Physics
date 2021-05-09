using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime;
using System.Diagnostics;


[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RayMarcherComponent : MonoBehaviour
{
    public ComputeShader Raymarcher;
    public ComputeShader Physics;
    public bool render = true;
    public bool physicsInEditor = true;
    public bool printAvgFrameRate = false;
    public bool physics = true;
    public bool cutShape = false;
    private bool set_cutout_set = false;
    RenderTexture target;
    Camera cam;
    List<ComputeBuffer> toDispose;
    ComputeBuffer shapeBuffer;
    
    const int Sphere = 0;
    const int Cube = 1;
    private int shapeCount;
    private bool shapesSet = false;

    void Init() {
        cam = Camera.current;
        QualitySettings.maxQueuedFrames = 0;
        Application.targetFrameRate = 10000;
        //UnityEngine.Profiling.Profiler.logFile = Application.persistentDataPath + "/log.txt";
        //UnityEngine.Profiling.Profiler.enableBinaryLog = true;
        //UnityEngine.Profiling.Profiler.enabled = true;
        //UnityEngine.Debug.Log(Application.persistentDataPath + "/log.txt");
    }

    ~RayMarcherComponent() {
        shapeBuffer.Dispose();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {

        Init();
        
        toDispose = new List<ComputeBuffer>();
        List<Shape> componentList = new List<Shape> (FindObjectsOfType<Shape> ());
        
        if (!shapesSet || !EditorApplication.isPlaying) {
            shapeCount = setBuffer<Shape, ShapeData>("shapes", "shapeCount");
            UnityEngine.Debug.Log(shapeCount);
            shapesSet = true;
        }

        InitTexture();
        SetParameters();
        
        //SetShapeBuffer();
        SetLightBuffer();

        //List<Shape> shapesInScene = new List<Shape> (FindObjectsOfType<Shape> ());
        

        //List<Light> lightsInScene = new List<Light> (FindObjectsOfType<Light>());
        //ComputeBuffer bool_buffer = new ComputeBuffer(1000, System.Runtime.InteropServices.Marshal.SizeOf<int>());
        //Physics.SetBuffer(0, "tasks", bool_buffer);
        //toDispose.Add(bool_buffer); 

        
        
        Raymarcher.SetTexture(0, "Source", source);
        Raymarcher.SetTexture(0, "Result", target);
        
        int threadGroupsX = Mathf.CeilToInt (cam.pixelWidth / 32.0f);
        int threadGroupsY = Mathf.CeilToInt (cam.pixelHeight / 32.0f);

        if (printAvgFrameRate && EditorApplication.isPlaying) {
            UnityEngine.Debug.Log(Time.frameCount / Time.time);
        }
        if ((EditorApplication.isPlaying || physicsInEditor) && physics) {
            
            //UnityEngine.Debug.Log(UnityEngine.QualitySettings.vSyncCount);
            Physics.SetFloat("deltaTime",  Time.deltaTime);
            Physics.SetInt("shapeCount", shapeCount);
            Physics.Dispatch(0, shapeCount, 1, 1);
            Physics.Dispatch(1, shapeCount, 1, 1);
        }
        if (render) {
            Raymarcher.Dispatch(0, threadGroupsX, threadGroupsY, 1);
            Graphics.Blit(target, destination);
        }
        

        foreach (var buffer in toDispose) {
            buffer.Dispose ();
        }
        
        if (!EditorApplication.isPlaying) {
            shapeBuffer.Dispose();
        }
        
    }
    
    void SetLightBuffer() {
        List<Light> lightsInScene = new List<Light> (FindObjectsOfType<Light>());
        
        LightData[] lightData = new LightData[lightsInScene.Count];
        for (int i = 0; i < lightData.Length; i++) {
            lightData[i].position = lightsInScene[i].GetComponent<Transform>().position;
            lightData[i].direction = new Vector3(0.0f, 0.0f, 0.0f);
        }
        
        ComputeBuffer lightBuffer = new ComputeBuffer (lightData.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(LightData)));
        lightBuffer.SetData(lightData);
        
        Raymarcher.SetBuffer(0, "lights", lightBuffer);
        Raymarcher.SetInt("lightCount", lightData.Length);
        
        toDispose.Add(lightBuffer);
    }

    //CURRENTLY WORKING ON THIS - FIX INHERITANCE ISSUES
    int setBuffer<B, BD>(string bufferName, string countName) 
    where B : UnityEngine.Object, Bufferable
    where BD : BufferDatatype {
        List<B> componentList = new List<B> (FindObjectsOfType<B> ());
        BD[] componentStruct = new BD[componentList.Count];

        for (int i = 0; i < componentStruct.Length; i++) {
            componentStruct[i] = (BD) componentList[i].toBufferable();
        }
        shapeBuffer = new ComputeBuffer (componentStruct.Length, System.Runtime.InteropServices.Marshal.SizeOf<BD>() );
        shapeBuffer.SetData (componentStruct);
        //MarchCompute.SetBuffer(0, bufferName, buffer);
        Shader.SetGlobalBuffer("shapes", shapeBuffer); 
        //buffer.Dispose();
        Raymarcher.SetInt(countName, componentStruct.Length);
        return componentStruct.Length;
    }

    void setBlendBuffer() {
        List<Blend> blendsInScene = new List<Blend> (FindObjectsOfType<Blend> ());
        BlendData[] blendData = new BlendData[blendsInScene.Count];
        for (int i = 0; i < blendData.Length; i++) {
            //blendData[i].blendType = (int) blendsInScene[i].GetComponent<Blend>().blendType;
            //blendData[i].shape1 = (int) blendsInScene[i].GetComponent<Blend>().shape1; //Sort out this to input shapes into the compute kernel
            //blendData[i].shape2 = (int) blendsInScene[i].GetComponent<Blend>().shape2;
        }
    }
    
    void InitTexture() {
        if (target == null || target.width != cam.pixelWidth || target.height == cam.pixelHeight) {
            if (target != null) {
                target.Release ();
            }
            target = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create ();
        }
    }
    
    void SetParameters() {
        Raymarcher.SetMatrix ("_CameraToWorld", cam.cameraToWorldMatrix);
        Raymarcher.SetMatrix ("_CameraInverseProjection", cam.projectionMatrix.inverse);
        Raymarcher.SetInt("cutting_shape", cutShape ? 1 : 0);
        Physics.SetInt("cutting_shape", cutShape ? 1 : 0);
        Physics.SetInt("set_cutout", 0);
        if (!set_cutout_set && cutShape) {
            UnityEngine.Debug.Log("egg");
            Physics.SetInt("set_cutout", 1);
            set_cutout_set = true;
        }
        
    }
    
    /*
    void SetShapes() {
        ShapeData[] shapes = new ShapeData[1];
        shapes[0] = new ShapeData () {
            position = testShapePosition,
            scale = testShapeScale,
            colour = testShapeColour
        };
        
        ComputeBuffer shapeBuffer = new ComputeBuffer(shapes.Length, System.Runtime.InteropServices.Marshal.SizeOf(shapes[0]));
        shapeBuffer.SetData(shapes);
        MarchCompute.SetBuffer(0, "shapes", shapeBuffer);
        MarchCompute.SetInt("shapeCount", shapes.Length);
        
        toDispose.Add(shapeBuffer);
    }*/
    
}
