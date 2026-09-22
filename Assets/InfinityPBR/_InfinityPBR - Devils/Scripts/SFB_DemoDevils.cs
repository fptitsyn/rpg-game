using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SFB_DemoDevils : MonoBehaviour {

	private Animator animator;
	public GameObject[] castObjects;
	public GameObject[] screamObjects;

	public GameObject[] Heads;
	public GameObject[] Horns;
	public Toggle[] Bodies;
	public Toggle[] TextureSets;
	public Toggle[] BodyTextures;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	public void Locomotion(float value){
		animator.SetFloat ("locomotion", value);
	}

	public void SuperRandom()
	{
		for (int i = 0; i < Heads.Length; i++)
		{
			Heads[i].SetActive(false);
		}
		for (int i = 0; i < Heads.Length; i++)
		{
			Horns[i].SetActive(false);
		}
		
		Heads[Random.Range(0, Heads.Length)].SetActive(true);
		Horns[Random.Range(0, Heads.Length)].SetActive(true);

		Bodies[Random.Range(0, Bodies.Length)].Select();
		RandomTextureSet();
	}

	public void RandomTextureSet() => TextureSets[Random.Range(0, TextureSets.Length)].Select();
	public void RandomBodyTexture() => BodyTextures[Random.Range(0, BodyTextures.Length)].Select();

	public void StartCast(){
		for (int i = 0; i < castObjects.Length; i++){
			if (castObjects [i].GetComponent<ParticleSystem> ()) {
				ParticleSystem ps = castObjects [i].GetComponent<ParticleSystem> ();
				var em = ps.emission;
				em.enabled = true;
			} else if (castObjects [i].GetComponent<Light> ()) {
				castObjects [i].GetComponent<Light> ().enabled = true;
			}
		}
	}

	public void StopCast(){
		for (int i = 0; i < castObjects.Length; i++){
			if (castObjects [i].GetComponent<ParticleSystem> ()) {
				ParticleSystem ps = castObjects [i].GetComponent<ParticleSystem> ();
				var em = ps.emission;
				em.enabled = false;
			} else if (castObjects [i].GetComponent<Light> ()) {
				castObjects [i].GetComponent<Light> ().enabled = false;
			}
		}
	}

	public void StartScream(){
		for (int i = 0; i < castObjects.Length; i++){
			if (screamObjects [i].GetComponent<ParticleSystem> ()) {
				ParticleSystem ps = castObjects [i].GetComponent<ParticleSystem> ();
				var em = ps.emission;
				em.enabled = true;
			} else if (screamObjects [i].GetComponent<Light> ()) {
				screamObjects [i].GetComponent<Light> ().enabled = true;
			}
		}
	}

	public void StopScream(){
		for (int i = 0; i < castObjects.Length; i++){
			if (screamObjects [i].GetComponent<ParticleSystem> ()) {
				ParticleSystem ps = castObjects [i].GetComponent<ParticleSystem> ();
				var em = ps.emission;
				em.enabled = false;
			} else if (screamObjects [i].GetComponent<Light> ()) {
				screamObjects [i].GetComponent<Light> ().enabled = false;
			}
		}
	}
}
