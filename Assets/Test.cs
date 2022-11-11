using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace InTheShadow
{
    public class Test : MonoBehaviour
    {
        public Texture2D texture;
        public ComputeShader rotateShader;
        public float timer;

        private Material _material;
        private RenderTexture _result;
        
        private int _kernel;

        // Start is called before the first frame update
        void Start()
        {
            _kernel = rotateShader.FindKernel ("CSMain");

            _material = GetComponent<Renderer>().material;

            _result = new RenderTexture (texture.width, texture.height, 0)
            {
                enableRandomWrite = true
            };
            _result.Create ();
            
            _material.mainTexture = _result;
            
            rotateShader.SetTexture(_kernel, "Result", _result);
            rotateShader.SetTexture(_kernel, "Texture", texture);
            rotateShader.SetFloat("Width", (float)texture.width);
            rotateShader.SetFloat("Height", (float)texture.height);
        }

        private float _t;
        private float _degrees;
        // Update is called once per frame
        void Update()
        {
            if (_t >= timer)
            {
                rotateShader.SetFloat("Sin", Mathf.Sin(_degrees));
                rotateShader.SetFloat("Cos", Mathf.Cos(_degrees));
                rotateShader.SetFloat("Rad", Mathf.Deg2Rad * _degrees);
                
                rotateShader.Dispatch (
                    _kernel, 
                    Mathf.CeilToInt(texture.width / 8f), 
                    Mathf.CeilToInt(texture.height / 8f), 
                    1);
                
                _t = 0.0f;
                _degrees += 5.0f;
            }

            _t += Time.deltaTime;
        }
    }
}
