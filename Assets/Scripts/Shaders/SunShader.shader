Shader "Custom/Effects_Sun" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}		
		_FireTex1 ("Fire Texture 1", 2D) = "black" {}
		_Tiling1 ("Fire Texture 1 Tiling", Vector) = (1,1,0,0)
		_FireTex2 ("Fire Texture 2", 2D) = "black" {}
		_Tiling2 ("Fire Texture 2 Tiling", Vector) = (1,1,0,0)
		_FlowTex ("Flow Map", 2D) = "grey" {}
		_FlowSpeed ("Flow Speed", Float ) = 0.3
		_WaveTex ("Wave Texture", 2D) = "black" {}
		_WaveTiling ("Wave Tiling", Vector) = (1,1,0,0)
		_Factor ("Factor", Float ) = 3.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		
		sampler2D _FireTex1;
		float4 _Tiling1;
		sampler2D _FireTex2;
		float4 _Tiling2;
		sampler2D _FlowTex;
		float _FlowSpeed;
		sampler2D _WaveTex;
		float4 _WaveTiling;
		float _Factor;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
		
			float2 uvCoords = IN.uv_MainTex;
		
			half4 mainTex = tex2D (_MainTex, uvCoords);
			float4 flowTex = tex2D (_FlowTex, uvCoords);
			flowTex.xy = flowTex.xy * 2.0 - 1.0;
			flowTex.y *= -1.0;			
			
			half4 waveTex = tex2D (_WaveTex, flowTex.zw * _WaveTiling.xy + _Time.y * _WaveTiling.zw );
			half wave = waveTex.x * 0.5 + 0.5;
			
			float scaledTime = _Time.y * _FlowSpeed + flowTex.z;
			
			float flowA = frac( scaledTime );
			float flowBlendA = 1.0 - abs( flowA * 2.0 - 1.0 );
			flowA -= 0.5;
			
			float flowB = frac( scaledTime + 0.5 );
			float flowBlendB = 1.0 - abs( flowB * 2.0 - 1.0 );
			flowB -= 0.5;
			
			half4 fireTex1 = tex2D (_FireTex1, uvCoords * _Tiling1.xy + _Time.y * _Tiling1.zw + ( flowTex.xy * flowA * 0.1 ) );
			half4 fireTex2 = tex2D (_FireTex2, uvCoords * _Tiling2.xy + _Time.y * _Tiling2.zw + ( flowTex.xy * flowB * 0.1 ) );
			
			half4 finalFire = lerp( fireTex1, fireTex2, flowBlendB );
			finalFire = lerp( mainTex.x, finalFire, mainTex.w );
			
			half4 Final = mainTex * finalFire * _Factor * wave;
			
			o.Albedo = 0.0;
			o.Alpha = 1.0;
			o.Emission = Final.xyz;
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}