using UnityEngine;
using System.Collections;

public class ParticleEffectManager : MonoBehaviour
{
    [Header("Particle Prefabs")]
    public GameObject dustParticlePrefab;
    public GameObject sparkleParticlePrefab;
    public GameObject explosionParticlePrefab;
    public GameObject healParticlePrefab;
    public GameObject speedBoostParticlePrefab;
    public GameObject invisibilityParticlePrefab;
    public GameObject spiderWebParticlePrefab;
    public GameObject exitGlowParticlePrefab;
    
    [Header("Effect Settings")]
    public float effectDuration = 2f;
    public int particleCount = 20;
    
    private static ParticleEffectManager instance;
    
    public static ParticleEffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ParticleEffectManager>();
                if (instance == null)
                {
                    GameObject particleManager = new GameObject("ParticleEffectManager");
                    instance = particleManager.AddComponent<ParticleEffectManager>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void PlayDustEffect(Vector3 position)
    {
        if (dustParticlePrefab != null)
        {
            StartCoroutine(SpawnParticleEffect(dustParticlePrefab, position, particleCount));
        }
    }
    
    public void PlaySparkleEffect(Vector3 position)
    {
        if (sparkleParticlePrefab != null)
        {
            StartCoroutine(SpawnParticleEffect(sparkleParticlePrefab, position, particleCount));
        }
    }
    
    public void PlayExplosionEffect(Vector3 position)
    {
        if (explosionParticlePrefab != null)
        {
            StartCoroutine(SpawnParticleEffect(explosionParticlePrefab, position, particleCount * 2));
        }
    }
    
    public void PlayHealEffect(Vector3 position)
    {
        if (healParticlePrefab != null)
        {
            StartCoroutine(SpawnParticleEffect(healParticlePrefab, position, particleCount));
        }
    }
    
    public void PlaySpeedBoostEffect(Vector3 position)
    {
        if (speedBoostParticlePrefab != null)
        {
            StartCoroutine(SpawnParticleEffect(speedBoostParticlePrefab, position, particleCount));
        }
    }
    
    public void PlayInvisibilityEffect(Vector3 position)
    {
        if (invisibilityParticlePrefab != null)
        {
            StartCoroutine(SpawnParticleEffect(invisibilityParticlePrefab, position, particleCount));
        }
    }
    
    public void PlaySpiderWebEffect(Vector3 position)
    {
        if (spiderWebParticlePrefab != null)
        {
            StartCoroutine(SpawnParticleEffect(spiderWebParticlePrefab, position, particleCount / 2));
        }
    }
    
    public void PlayExitGlowEffect(Vector3 position)
    {
        if (exitGlowParticlePrefab != null)
        {
            StartCoroutine(SpawnParticleEffect(exitGlowParticlePrefab, position, particleCount));
        }
    }
    
    IEnumerator SpawnParticleEffect(GameObject particlePrefab, Vector3 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            );
            
            GameObject particle = Instantiate(particlePrefab, position + randomOffset, Quaternion.identity);
            
            // Add random rotation
            particle.transform.Rotate(0, 0, Random.Range(0f, 360f));
            
            // Add random scale
            float randomScale = Random.Range(0.5f, 1.5f);
            particle.transform.localScale = Vector3.one * randomScale;
            
            // Destroy after duration
            Destroy(particle, effectDuration);
            
            yield return new WaitForSeconds(0.05f); // Small delay between particles
        }
    }
    
    public void PlayTrailEffect(Transform target, GameObject trailPrefab, float duration)
    {
        StartCoroutine(CreateTrailEffect(target, trailPrefab, duration));
    }
    
    IEnumerator CreateTrailEffect(Transform target, GameObject trailPrefab, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration && target != null)
        {
            if (trailPrefab != null)
            {
                GameObject trail = Instantiate(trailPrefab, target.position, Quaternion.identity);
                Destroy(trail, 1f);
            }
            
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void PlayAreaEffect(Vector3 center, float radius, GameObject effectPrefab, int count)
    {
        StartCoroutine(SpawnAreaEffect(center, radius, effectPrefab, count));
    }
    
    IEnumerator SpawnAreaEffect(Vector3 center, float radius, GameObject effectPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            Vector3 spawnPosition = center + new Vector3(randomPoint.x, randomPoint.y, 0);
            
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, spawnPosition, Quaternion.identity);
                Destroy(effect, effectDuration);
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}
