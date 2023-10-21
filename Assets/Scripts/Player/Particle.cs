using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Particle : MonoBehaviour
{
    public List<Texture2D> flash;
    public List<Texture2D> fire;
    public List<Texture2D> wooden;

    public void Play(string skillName)
    {
        Texture2D texture;
        if(skillName == "flashhit"){
            texture = flash[Random.Range(0, flash.Count)];
        }else if(skillName == "firehit"){
            texture = fire[Random.Range(0, fire.Count)];
        }else if(skillName == "woodenhit"){
            texture = wooden[Random.Range(0, wooden.Count)];
        }else{
            throw new System.Exception("Unknown particle type: " + skillName);
        }

        ParticleSystem ps = GetComponent<ParticleSystem>();
        ParticleSystemRenderer psr = GetComponent<ParticleSystemRenderer>();
        psr.material.mainTexture = texture;
        ps.Play();
    }
}
