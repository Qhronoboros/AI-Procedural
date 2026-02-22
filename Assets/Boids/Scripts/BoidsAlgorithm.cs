using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
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

    private List<BoidClass> _boids = new List<BoidClass>();
    private Boid[] boids;

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

    class BoidClass
    {
        public BoidClass(GameObject gameObject, Vector3 position, Vector3 velocity)
        {
            this.gameObject = gameObject;
            this.position = position;
            this.velocity = velocity;
        }

        public GameObject gameObject;
        public Vector3 position;
        public Vector3 velocity;
    }

    static Vector3 GetRandomPointInsideSphere(float minimum, float maximum)
    {
        Vector3 randomUnitPoint = Random.insideUnitSphere;
        Vector3 normalizedVector = Vector3.Normalize(randomUnitPoint);

        return randomUnitPoint * (maximum - minimum) + normalizedVector * minimum;
    }

    private void Start()
    {
        boids = new Boid[boidStartAmount];
    
        for (int i = 0; i < boidStartAmount; i++)
        {
            // Set positions and velocity
            Vector3 randomPosition = GetRandomPointInsideSphere(_startDistanceMin, _startDistanceMax);
            Vector3 randomVelocity = GetRandomPointInsideSphere(_startVelocityMin, _startVelocityMax);

            _boids.Add(new BoidClass(Instantiate(boidPrefab, randomPosition, Quaternion.identity), randomPosition, randomVelocity));


            boids[i] = new Boid(randomPosition, randomVelocity);
        }
    }

    private void ComputeBoids()
    {
        int kernel = computeShader.FindKernel("CSMain");
    
        ComputeBuffer boidsBuffer = new ComputeBuffer(boids.Length, sizeof(float) * 6);
        boidsBuffer.SetData(boids);
        
        computeShader.SetBuffer(kernel, "boids", boidsBuffer);
        
        computeShader.SetFloat("cohesionMultiplier", _cohesionMultiplier);
        
        computeShader.SetFloat("separationDistance", _separationDistance);
        computeShader.SetFloat("separationMultiplier", _separationMultiplier);
        
        computeShader.SetFloat("alignmentMultiplier", _alignmentMultiplier);
        
        computeShader.SetFloat("velocityMax", _velocityMax);
        
        computeShader.Dispatch(kernel, boids.Length / 8, 1, 1);
        
        boidsBuffer.GetData(boids);
        
        for (int i = 0; i < boids.Length; i++)
        {
            _boids[i].gameObject.transform.position = boids[i].position;
        }
    }

    private void Update()
    {
        ComputeBoids();
    }

    private void FixedUpdate()
    {
        return;
        
        Vector3 v1, v2, v3, v4;
        int boidsCount = _boids.Count;

        // for (int i = 0; i < boidsCount; i++)
        // {
        //     _boids[i].gameObject.transform.position = _boids[i].position;
        //     Debug.Log(_boids[i].position);
        // }

        for (int i = 0; i < boidsCount; i++)
        {  
            BoidClass boid = _boids[i];

            // Cohesion
            v1 = Cohesion(boid);
            // Separation
            v2 = Separation(boid);
            // Alignment
            v3 = Alignment(boid);

            // Bound Position
            v4 = BoundPosition(boid);

            // Add velocities together
            boid.velocity += v1 + v2 + v3 + v4;

            // Limit Velocity
            LimitVelocity(boid);

            // Change position using new velocity
            boid.position += boid.velocity * Time.deltaTime;
            boid.gameObject.transform.position = boid.position;
        }
    }

    private Vector3 Cohesion(BoidClass currentBoid)
    {
        Vector3 positionSum = Vector3.zero;

        foreach(BoidClass boid in _boids)
        {
            if (boid.gameObject != currentBoid.gameObject)
            {
                positionSum += boid.position;
            }
        }

        Vector3 positionMean = positionSum / Mathf.Max(_boids.Count - 1, 1);

        return (positionMean - currentBoid.position) * _cohesionMultiplier;
    }

    private Vector3 Separation(BoidClass currentBoid)
    {
        Vector3 velocity = Vector3.zero;

        foreach(BoidClass boid in _boids)
        {
            if (boid.gameObject != currentBoid.gameObject)
            {
                if (Vector3.Distance(boid.position, currentBoid.position) < _separationDistance)
                {
                    velocity -= boid.position - currentBoid.position;
                }
            }
        }

        return velocity * _separationMultiplier;
    }

    private Vector3 Alignment(BoidClass currentBoid)
    {
        Vector3 velocity = Vector3.zero;

        foreach(BoidClass boid in _boids)
        {
            if (boid.gameObject != currentBoid.gameObject)
            {
                velocity += boid.velocity;
            }
        }

        Vector3 velocityMean = velocity / Mathf.Max(_boids.Count - 1, 1);

        return (velocityMean - currentBoid.velocity) * _alignmentMultiplier;
    }

    private void LimitVelocity(BoidClass currentBoid)
    {
        if (currentBoid.velocity.magnitude > _velocityMax)
        {
            currentBoid.velocity = currentBoid.velocity / Mathf.Max(currentBoid.velocity.magnitude, 0.00001f) * _velocityMax;
        }
    }

    private Vector3 BoundPosition(BoidClass currentBoid)
    {
        Vector3 velocity = Vector3.zero;

        if (currentBoid.position.x < -40.0f)
        {
            velocity.x = 1;
        }
        else if (currentBoid.position.x > 40.0f)
        {
            velocity.x = -1;
        }

        if (currentBoid.position.y < -20.0f)
        {
            velocity.y = 1;
        }
        else if (currentBoid.position.y > 20.0f)
        {
            velocity.y = -1;
        }

        if (currentBoid.position.z < -20.0f)
        {
            velocity.z = 1;
        }
        else if (currentBoid.position.z > 20.0f)
        {
            velocity.z = -1;
        }

        return velocity;
    }
}
