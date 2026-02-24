using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    [SerializeField] private bool _useComputeShader;

    [SerializeField] private ComputeShader computeShader;

    [SerializeField] private GameObject boidPrefab;

    [Header("Start Values")]
    [SerializeField] private int boidStartAmount = 20;

    [SerializeField] private float _startDistanceMin = 0.0f;
    [SerializeField] private float _startDistanceMax = 100.0f;

    [SerializeField] private float _startVelocityMin = 5.0f;
    [SerializeField] private float _startVelocityMax = 10.0f;

    [Header("Rule Values")]
    [SerializeField] private float _cohesionMultiplier = 0.01f;
    [SerializeField] private float _separationDistance = 5.0f;
    [SerializeField] private float _separationMultiplier = 0.01f;
    [SerializeField] private float _alignmentMultiplier = 0.01f;

    [SerializeField] private float _velocityMax = 20.0f;

    [Header("Bounds")]
    [SerializeField] private float _leftBounds = 40.0f;
    [SerializeField] private float _rightBounds = 40.0f;
    [SerializeField] private float _topBounds = 20.0f;
    [SerializeField] private float _bottomBounds = 20.0f;
    [SerializeField] private float _frontBounds = 20.0f;
    [SerializeField] private float _backBounds = 20.0f;
    [SerializeField] private float _boundsMultiplier = 1.0f;


    private List<UnityBoid> _unityBoids = new List<UnityBoid>();
    private Boid[] _boids;

    public struct Boid
    {
        public Boid(Vector3 position, Vector3 velocity)
        {
            this.position = position;
            this.velocity = velocity;
        }

        public Vector3 position;
        public Vector3 velocity;
    }   

    class UnityBoid
    {
        public UnityBoid(GameObject gameObject, Boid boid)
        {
            this.gameObject = gameObject;
            this.boid = boid;
        }

        public GameObject gameObject;
        public Boid boid;
    }

    static Vector3 GetRandomPointInsideSphere(float minimum, float maximum)
    {
        Vector3 randomUnitPoint = Random.insideUnitSphere;
        Vector3 normalizedVector = Vector3.Normalize(randomUnitPoint);

        return randomUnitPoint * (maximum - minimum) + normalizedVector * minimum;
    }

    private void Start()
    {
        _boids = new Boid[boidStartAmount];
    
        for (int i = 0; i < boidStartAmount; i++)
        {
            // Set positions and velocity
            Vector3 randomPosition = GetRandomPointInsideSphere(_startDistanceMin, _startDistanceMax);
            Vector3 randomVelocity = GetRandomPointInsideSphere(_startVelocityMin, _startVelocityMax);

            Boid boid = _boids[i] = new Boid(randomPosition, randomVelocity);
            _unityBoids.Add(new UnityBoid(Instantiate(boidPrefab, randomPosition, Quaternion.identity), boid));
        }
    }

    private void ComputeBoids()
    {
        int kernel = computeShader.FindKernel("CSMain");
    
        ComputeBuffer boidsBuffer = new ComputeBuffer(_boids.Length, sizeof(float) * 6);
        boidsBuffer.SetData(_boids);
        
        computeShader.SetBuffer(kernel, "boids", boidsBuffer);
        
        computeShader.SetFloat("cohesionMultiplier", _cohesionMultiplier);
        
        computeShader.SetFloat("separationDistance", _separationDistance);
        computeShader.SetFloat("separationMultiplier", _separationMultiplier);
        
        computeShader.SetFloat("alignmentMultiplier", _alignmentMultiplier);
        
        computeShader.SetFloat("velocityMax", _velocityMax);
        
        computeShader.SetFloat("leftBounds", _leftBounds);
        computeShader.SetFloat("rightBounds", _rightBounds);
        computeShader.SetFloat("topBounds", _topBounds);
        computeShader.SetFloat("bottomBounds", _bottomBounds);
        computeShader.SetFloat("frontBounds", _frontBounds);
        computeShader.SetFloat("backBounds", _backBounds);
        computeShader.SetFloat("boundsMultiplier", _boundsMultiplier);
        
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        
        computeShader.Dispatch(kernel, _boids.Length / 8, 1, 1);
        
        boidsBuffer.GetData(_boids);

        for (int i = 0; i < _boids.Length; i++)
        {
            _unityBoids[i].gameObject.transform.position = _boids[i].position;
        }
        
        boidsBuffer.Dispose();
    }

    private void Update()
    {
        if (_useComputeShader)
        {
            ComputeBoids();
        }
        else
        {
            CalculateBoids();
        }
    }

    private void CalculateBoids()
    {
        Vector3 v1, v2, v3, v4;
        int boidsCount = _unityBoids.Count;

        // for (int i = 0; i < boidsCount; i++)
        // {
        //     _boids[i].gameObject.transform.position = _boids[i].position;
        //     Debug.Log(_boids[i].position);
        // }

        for (int i = 0; i < boidsCount; i++)
        {  
            UnityBoid unityBoid = _unityBoids[i];
            Boid boid = unityBoid.boid;

            // Cohesion
            v1 = Cohesion(unityBoid);
            // Separation
            v2 = Separation(unityBoid);
            // Alignment
            v3 = Alignment(unityBoid);

            // Bound Position
            v4 = BoundPosition(unityBoid);

            // Add velocities together
            boid.velocity += v1 + v2 + v3 + v4;

            // Limit Velocity
            LimitVelocity(unityBoid);

            // Change position using new velocity
            boid.position += boid.velocity * Time.deltaTime;
            unityBoid.gameObject.transform.position = boid.position;
            
            unityBoid.boid = boid;
        }
    }

    private Vector3 Cohesion(UnityBoid currentUnityBoid)
    {
        Vector3 positionSum = Vector3.zero;

        foreach(UnityBoid unityBoid in _unityBoids)
        {
            if (unityBoid.gameObject != currentUnityBoid.gameObject)
            {
                positionSum += unityBoid.boid.position;
            }
        }

        Vector3 positionMean = positionSum / Mathf.Max(_unityBoids.Count - 1, 1);

        return (positionMean - currentUnityBoid.boid.position) * _cohesionMultiplier;
    }

    private Vector3 Separation(UnityBoid currentUnityBoid)
    {
        Vector3 velocity = Vector3.zero;

        foreach(UnityBoid unityBoid in _unityBoids)
        {
            if (unityBoid.gameObject != currentUnityBoid.gameObject)
            {
                if (Vector3.Distance(unityBoid.boid.position, currentUnityBoid.boid.position) < _separationDistance)
                {
                    velocity -= unityBoid.boid.position - currentUnityBoid.boid.position;
                }
            }
        }

        return velocity * _separationMultiplier;
    }

    private Vector3 Alignment(UnityBoid currentUnityBoid)
    {
        Vector3 velocity = Vector3.zero;

        foreach(UnityBoid unityBoid in _unityBoids)
        {
            if (unityBoid.gameObject != currentUnityBoid.gameObject)
            {
                velocity += unityBoid.boid.velocity;
            }
        }

        Vector3 velocityMean = velocity / Mathf.Max(_unityBoids.Count - 1, 1);

        return (velocityMean - currentUnityBoid.boid.velocity) * _alignmentMultiplier;
    }

    private void LimitVelocity(UnityBoid currentUnityBoid)
    {
        Boid boid = currentUnityBoid.boid;
    
        if (boid.velocity.magnitude > _velocityMax)
        {
            boid.velocity = boid.velocity / Mathf.Max(boid.velocity.magnitude, 0.00001f) * _velocityMax;
        }
        
        currentUnityBoid.boid = boid;
    }

    private Vector3 BoundPosition(UnityBoid currentUnityBoid)
    {
        Vector3 velocity = Vector3.zero;
        Boid boid = currentUnityBoid.boid;

        if (boid.position.x < -_leftBounds)
        {
            velocity.x = _boundsMultiplier;
        }
        else if (boid.position.x > _rightBounds)
        {
            velocity.x = -_boundsMultiplier;
        }

        if (boid.position.y < -_bottomBounds)
        {
            velocity.y = _boundsMultiplier;
        }
        else if (boid.position.y > _topBounds)
        {
            velocity.y = -_boundsMultiplier;
        }

        if (boid.position.z < -_backBounds)
        {
            velocity.z = _boundsMultiplier;
        }
        else if (boid.position.z > _frontBounds)
        {
            velocity.z = -_boundsMultiplier;
        }

        return velocity;
    }
}
