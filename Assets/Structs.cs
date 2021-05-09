using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface BufferDatatype {};
public struct Matrix3x3 {
    public float a00;
    public float a01;
    public float a02;
    public float a10;
    public float a11;
    public float a12;
    public float a20;
    public float a21;
    public float a22;

}
public struct ShapeData : BufferDatatype {
    public Matrix4x4 transform;
    public int shapeType;
    public Vector4 colour;
    public Vector3 scale;
    public float ambient;
    public float diffuse;
    public float specular;
    public int shininess;
    public float transparency;
    public Vector3 velocity;
    public Matrix4x4 inverseTransform;
    public int hasPhysics;
    public int reflective;
    public Vector3 acceleration;
    public Vector3 collision_push;
    public float elasticity;
    public float mass;
    public float gravity;
    public int colliding_obj;
    public Vector3 colliding_obj_norm;
    public float deepest_collision;
    public float drag_coeff;
    public Vector3 rotation_direction;
    public Vector3 rotation_position;
    public Vector3 angular_velocity;
    public float angular_acceleration;
    public Vector3 angular_momentum;
    public Vector3 torque;
    public Vector3 force;
    public Vector3 impulse;
    public Vector3 deepest_collision_position;
    public Matrix3x3 orientation;
    public Matrix3x3 inverse_world_inertia_tensor;
    public Matrix3x3 inverse_body_inertia_tensor;
    public float collision_volume;
    public Vector3 previous_pos;
    public int hit_shape;
}

public struct LightData : BufferDatatype {
    public Vector3 position;
    public Vector3 direction;
}

public struct BlendData : BufferDatatype {
    public int blendType;
    public int shape1;
    public int shape2;
}