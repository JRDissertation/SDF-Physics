using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BlendType {
    None,
    Subtract
}

public class Shape : MonoBehaviour, Bufferable
{
    
    public enum ShapeType {
        Sphere,
        Cube,
        Torus,
        MandelBulb,
        Plane,
        HexagonalPrism,
        RepeatingCubes,
        CappedTorus,
        Cone,
        TriangularPrism,
        Cylinder
    }
    
    public BufferDatatype toBufferable() {
        ShapeData sd;
        sd.transform = this.transform.localToWorldMatrix;
        sd.shapeType = (int) this.type;
        sd.colour = this.colour;
        sd.scale = this.transform.localScale;
        sd.ambient = this.ambient;
        sd.diffuse = this.diffuse;
        sd.specular = this.specular;
        sd.shininess = this.shininess;
        sd.transparency = this.transparency;
        sd.velocity = this.velocity;
        sd.inverseTransform = this.transform.worldToLocalMatrix;
        sd.hasPhysics = this.physicsEnabled ? 1 : 0;
        sd.reflective = this.reflective ? 1 : 0;
        sd.acceleration = this.acceleration;
        sd.collision_push = new Vector3(0.0f, 0.0f, 0.0f);
        sd.elasticity = this.elasticity;
        sd.mass = this.mass;
        sd.gravity = this.gravity;
        sd.colliding_obj = -1;
        sd.colliding_obj_norm = new Vector3(0.0f, 0.0f, 0.0f);
        sd.deepest_collision = 0.0f;
        sd.drag_coeff = 0.0f;
        sd.rotation_direction = new Vector3(0.0f, 0.0f, 0.0f);
        sd.rotation_position = new Vector3(0.0f, 0.0f, 0.0f);
        sd.angular_velocity = this.angularVelocity;
        sd.angular_acceleration = 0.0f;
        sd.angular_momentum = this.angularMomentum;
        sd.torque = new Vector3(0.0f, 0.0f, 0.0f);
        sd.force = new Vector3(0.0f, 0.0f, 0.0f);
        sd.impulse = new Vector3(0.0f, 0.0f, 0.0f);
        sd.deepest_collision_position = new Vector3(0.0f, 0.0f, 0.0f);
        Matrix3x3 identity;
        identity.a00 = 1.0f;
        identity.a01 = 0.0f;
        identity.a02 = 0.0f;
        identity.a10 = 0.0f;
        identity.a11 = 1.0f;
        identity.a12 = 0.0f;
        identity.a20 = 0.0f;
        identity.a21 = 0.0f;
        identity.a22 = 0.0f;
        Matrix3x3 boxInertiaTensor;
        boxInertiaTensor.a00 = 1.0f / 0.16666666f;
        boxInertiaTensor.a01 = 0.0f;
        boxInertiaTensor.a02 = 0.0f;
        boxInertiaTensor.a10 = 0.0f;
        boxInertiaTensor.a11 = 1.0f / 0.16666666f;
        boxInertiaTensor.a12 = 0.0f;
        boxInertiaTensor.a20 = 0.0f;
        boxInertiaTensor.a21 = 0.0f;
        boxInertiaTensor.a22 = 1.0f / 0.16666666f;
        
        sd.orientation = identity;
        sd.inverse_world_inertia_tensor = identity;
        sd.inverse_body_inertia_tensor = getInertiaTensor(this.type);
        sd.collision_volume = 0.0f;
        sd.previous_pos = new Vector3(0.0f, 0.0f, 0.0f);
        sd.hit_shape = -1;
        return sd;
    }
    private Matrix3x3 getInertiaTensor(ShapeType shapeType) {
        Matrix3x3 inertiaTensor;
        if (shapeType == ShapeType.Cube) {
            inertiaTensor.a00 = (this.mass * (this.transform.localScale.y * this.transform.localScale.y + this.transform.localScale.z * this.transform.localScale.z))/12.0f;
            inertiaTensor.a01 = 0.0f;
            inertiaTensor.a02 = 0.0f;
            inertiaTensor.a10 = 0.0f;
            inertiaTensor.a11 = (this.mass * (this.transform.localScale.x * this.transform.localScale.x + this.transform.localScale.z * this.transform.localScale.z))/12.0f;
            inertiaTensor.a12 = 0.0f;
            inertiaTensor.a20 = 0.0f;
            inertiaTensor.a21 = 0.0f;
            inertiaTensor.a22 = (this.mass * (this.transform.localScale.x * this.transform.localScale.x + this.transform.localScale.y * this.transform.localScale.y))/12.0f;
        }
        else if (shapeType == ShapeType.Sphere) {
            inertiaTensor.a00 = (this.mass * (this.transform.localScale.y * this.transform.localScale.y + this.transform.localScale.z * this.transform.localScale.z))/5.0f;
            inertiaTensor.a01 = 0.0f;
            inertiaTensor.a02 = 0.0f;
            inertiaTensor.a10 = 0.0f;
            inertiaTensor.a11 = (this.mass * (this.transform.localScale.x * this.transform.localScale.x + this.transform.localScale.z * this.transform.localScale.z))/5.0f;
            inertiaTensor.a12 = 0.0f;
            inertiaTensor.a20 = 0.0f;
            inertiaTensor.a21 = 0.0f;
            inertiaTensor.a22 = (this.mass * (this.transform.localScale.x * this.transform.localScale.x + this.transform.localScale.y * this.transform.localScale.y))/5.0f;
        }
        else if (shapeType == ShapeType.Cone) {
            float approx_r = 3.4f / 5.8f;
            inertiaTensor.a00 = (this.mass / 20.0f) * ((2.0f * (2.0f * this.transform.localScale.y) * (2.0f * this.transform.localScale.y)) + (approx_r * approx_r));
            inertiaTensor.a01 = 0.0f;
            inertiaTensor.a02 = 0.0f;
            inertiaTensor.a10 = 0.0f;
            inertiaTensor.a11 = (this.mass / 20.0f) * ((2.0f * (2.0f * this.transform.localScale.y) * (2.0f * this.transform.localScale.y)) + (approx_r * approx_r));
            inertiaTensor.a12 = 0.0f;
            inertiaTensor.a20 = 0.0f;
            inertiaTensor.a21 = 0.0f;
            inertiaTensor.a22 = 0.3f * this.mass * approx_r * approx_r;
        }
        else if (shapeType == ShapeType.Cylinder) {
            float approx_r = (this.transform.localScale.x + this.transform.localScale.z) / 2.0f;
            inertiaTensor.a00 = (1.0f / 12.0f) * this.mass * (3.0f * approx_r * approx_r + this.transform.localScale.y * this.transform.localScale.y);
            inertiaTensor.a01 = 0.0f;
            inertiaTensor.a02 = 0.0f;
            inertiaTensor.a10 = 0.0f;
            inertiaTensor.a11 = (1.0f / 12.0f) * this.mass * (3.0f * approx_r * approx_r + this.transform.localScale.y * this.transform.localScale.y);
            inertiaTensor.a12 = 0.0f;
            inertiaTensor.a20 = 0.0f;
            inertiaTensor.a21 = 0.0f;
            inertiaTensor.a22 = 0.5f * this.mass * approx_r * approx_r;
        }
        else {
            //Default case gives 1x1 cube
            //UnityEngine.Debug.Log("Warning: Using default cube inertia tensor");
            inertiaTensor.a00 = 1.0f / 0.16666666f;
            inertiaTensor.a01 = 0.0f;
            inertiaTensor.a02 = 0.0f;
            inertiaTensor.a10 = 0.0f;
            inertiaTensor.a11 = 1.0f / 0.16666666f;
            inertiaTensor.a12 = 0.0f;
            inertiaTensor.a20 = 0.0f;
            inertiaTensor.a21 = 0.0f;
            inertiaTensor.a22 = 1.0f / 0.16666666f;
        }
        return inertiaTensor;
    }
    [Header("Rendering")]
    public ShapeType type;
    public Color colour;
    [Range(0.0f, 1.0f)]
    public float ambient = 0.33f;
    [Range(0.0f, 1.0f)]
    public float diffuse = 0.33f;
    public bool reflective = false;
    private float specular = 0.33f;
    private int shininess = 20;
    private float transparency = 0.0f;
    [Header("Physics")]
    public Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 force = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 acceleration = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 angularMomentum = new Vector3(0.0f, 0.0f, 0.0f);
    [Range(0.0f, 1.0f)]
    public float elasticity = 0.75f;
    [Range(0.0f, 10.0f)]
    public float mass = 1.0f;
    [Range(0.0f, 10.0f)]
    public float gravity = 1.0f;
    public bool physicsEnabled = true;
}