using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesAttractorScript : MonoBehaviour {

	public float pullDistance = 200;
	public Transform magnetPoint;

	ParticleSystem partSystem;

	void Start() {
		partSystem = gameObject.GetComponent<ParticleSystem>();
		magnetPoint = GameObject.FindWithTag ("Player").transform;
	}

	public void StartParticles() {
		partSystem.Play ();
	}

	public void StopParticles() {
		partSystem.Stop ();
	}

	void Update () {
		ParticlePull ();
	}

	void ParticlePull() {
		float sqrPullDistance = pullDistance * pullDistance;

		ParticleSystem.Particle[] x = new ParticleSystem.Particle[partSystem.particleCount];
		int y = partSystem.GetParticles (x);

		for (int i = 0; i < y; i++) {
			Vector3 offset = magnetPoint.position - transform.position;
			float sqrLen = offset.sqrMagnitude;
			if (sqrLen <= sqrPullDistance) {
				x [i].position = Vector3.Lerp (x [i].position, offset * 2, Mathf.SmoothStep (0, 2, (Time.deltaTime / 0.2f)));
				if ((x[i].position - offset * 2).magnitude <= 2) {
					x [i].remainingLifetime = 0;
				}
			}
		}

		partSystem.SetParticles (x, y);
		return;
	}
}
