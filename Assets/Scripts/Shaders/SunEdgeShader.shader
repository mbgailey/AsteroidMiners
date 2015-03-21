Shader "Custom/Effects_Sun_Edge" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTiling1 ("Base 1 Tiling", Vector) = (1,1,0,0)
		_MainTiling2 ("Base 2 Tiling", Vector) = (1,1,0,0)
		_FireTex1 ("Fire Texture 1", 2D) = "black" {}
		_Tiling1 ("Fire Texture 1 Tiling", Vector) = (1,1,0,0)
		_FireTex2 ("Fire Texture 2", 2D) = "black" {}
		_Tiling2 ("Fire Texture 2 Tiling", Vector) = (1,1,0,0)
		_FireTex3 ("Fire Texture 3", 2D) = "black" {}
		_Tiling3 ("Fire Texture 3 Tiling", Vector) = (1,1,0,0)
		_Factor ("Factor", Float ) = 3.0
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="False"
			"RenderType"="Transparent"
		}
		LOD 200
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float4 _MainTiling1;
		float4 _MainTiling2;
		sampler2D _FireTex1;
		float4 _Tiling1;
		sampler2D _FireTex2;
		float4 _Tiling2;
		sampler2D _FireTex3;
		float4 _Tiling3;
		float _Factor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 mainTex = tex2D (_MainTex, IN.uv_MainTex * _MainTiling1.xy + _Time.y * _MainTiling1.zw );
			half4 mainTex2 = tex2D (_MainTex, IN.uv_MainTex * _MainTiling2.xy + _Time.y * _MainTiling2.zw );
			half4 fireTex1 = tex2D (_FireTex1, IN.uv_MainTex * _Tiling1.xy + _Time.y * _Tiling1.zw );
			half4 fireTex2 = tex2D (_FireTex2, IN.uv_MainTex * _Tiling2.xy + _Time.y * _Tiling2.zw );
			half4 fireTex3 = tex2D (_FireTex3, IN.uv_MainTex * _Tiling3.xy + _Time.y * _Tiling3.zw );
			
			mainTex = ( mainTex2 + mainTex ) * 0.5;
			float4 edge = mainTex * mainTex;
			mainTex *= fireTex1.xxxx + fireTex2.xxxx + fireTex3.xxxx * 0.3;
			mainTex += edge;
			mainTex.xyz *= _Factor;
			
			o.Albedo = 0.0;
			o.Emission = mainTex;
			o.Alpha = saturate( mainTex.w );
		}
		ENDCG
	} 
	FallBack "Diffuse"
}