Shader "Custom/Effects_Sun_Flare" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Factor ("Factor", Float ) = 3.0
	}
	SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		LOD 200
		Cull Off
		ZWrite Off
		ZTest LEqual
		Blend SrcAlpha One
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		float _Factor;

		struct Input {
			float2 uv_MainTex;
			float fade;
		};
		
		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			float scaledTime = _Time.y * 0.03 * ( v.color.x * 0.2 + 0.9 );
			float fracTime = frac( scaledTime + v.color.x );
			float expand = lerp( 0.6, 1.0, fracTime );
			float fade = smoothstep( 0.0, 0.2, fracTime ) * smoothstep( 1.0, 0.5, fracTime );
			v.vertex.xyz *= expand;
			o.fade = fade;
		
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half4 mainTex = tex2D (_MainTex, IN.uv_MainTex);
					
			o.Albedo = 0.0;
			o.Emission = mainTex * _Factor;
			o.Alpha = mainTex.w * IN.fade;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}