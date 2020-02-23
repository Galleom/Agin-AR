Shader "Custom/ToonShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.4,0.4,0.4,1)
		_LightStep("Light Step", Range(0,1)) = 0.01
        [HDR]
        _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _Glossiness("Glossiness", Float) = 32
        [HDR]
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 0.716
        _RimThreshold("Rim Threshold", Range(0, 1)) = 0.1
		_RimStep("Rim Step", Range(0,1)) = 0.01
    }
    SubShader
    {
        Tags { 
            "LightMode" = "ForwardBase"
            "PassFlags" = "OnlyDirectional"
        }
        
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;				
				float4 uv : TEXCOORD0;
                float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD1;
                SHADOW_COORDS(2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _AmbientColor;
            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;
            float _LightStep;
            float _RimStep;
            float _RimThreshold;
            
			v2f vert(appdata_t IN)
			{ 
				v2f OUT;
				OUT.pos = UnityObjectToClipPos(IN.vertex);
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
                OUT.viewDir = WorldSpaceViewDir(IN.vertex);
				return OUT;
			}
			float4 _Color;

			fixed4 frag(v2f IN) : SV_Target
			{ 

                float3 normal = normalize(IN.worldNormal);
                float3 viewDir = normalize(IN.viewDir);

				// Lighting below is calculated using Blinn-Phong,
				// with values thresholded to creat the "toon" look.
				// https://en.wikipedia.org/wiki/Blinn-Phong_shading_model

				// Calculate illumination from directional light.
				// _WorldSpaceLightPos0 is a vector pointing the OPPOSITE
				// direction of the main directional light.
                float NdotL = dot(_WorldSpaceLightPos0, normal);
				// Samples the shadow map, returning a value in the 0...1 range,
				// where 0 is in the shadow, and 1 is not.
                float shadow = SHADOW_ATTENUATION(IN);
				// Partition the intensity into light and dark, smoothly interpolated
				// between the two to avoid a jagged break.
                float lightIntensity = smoothstep(0, _LightStep, NdotL * shadow);
				// Multiply by the main directional light's intensity and color.
                float4 light = lightIntensity * _LightColor0;
                
				// Calculate specular reflection.
                float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
                float NdotH = dot(normal, halfVector);
				// Multiply _Glossiness by itself to allow artist to use smaller
				// glossiness values in the inspector.
                float specularIntensity = pow(NdotH * lightIntensity, _Glossiness * _Glossiness);
                float specularIntensitySmooth = smoothstep(0.005, 0.01, specularIntensity);
                float4 specular = specularIntensitySmooth * _SpecularColor;
                
				// Calculate rim lighting.
                float rimDot = 1 - dot(viewDir, normal);
				// We only want rim to appear on the lit side of the surface,
				// so multiply it by NdotL, raised to a power to smoothly blend it.
                float rimIntensity = rimDot * pow(NdotH * lightIntensity, _RimThreshold);
                rimIntensity = smoothstep(_RimAmount - _RimStep, _RimAmount + _RimStep, rimIntensity);
                
                float4 rim = rimIntensity * _RimColor;

				float4 sample = tex2D(_MainTex, IN.uv);
                
				return _Color * sample * ( _AmbientColor + light + specular + rim);
			}
			ENDCG
		}
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    FallBack "Diffuse"
}
