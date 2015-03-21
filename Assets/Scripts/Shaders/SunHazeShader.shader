Shader "Custom/Effects_Sun_Haze" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Factor ("Factor", Float ) = 3.0
		_DistortTex ("Distort Texture", 2D) = "white" {}
		_Tiling1 ("Distort 1 Tiling", Vector) = (1,1,0,0)
		_Tiling2 ("Distort 2 Tiling", Vector) = (1,1,0,0)
		_Tiling3 ("Distort 3 Tiling", Vector) = (1,1,0,0)
		_Distortion ("Distortion", Range (0.01, 1)) = 0.2
	}
	
	CGINCLUDE	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float4 screenPos : TEXCOORD1;
	};
	
	sampler2D _MainTex;
	float4 _Color;
	float _Factor;
	
	sampler2D _DistortTex;
	float4 _Tiling1;
	float4 _Tiling2;
	float4 _Tiling3;
	float _Distortion;
	
	sampler2D _GrabTexture;// : register(s0);
	
	v2f vertMain( appdata_full v ) { 
		v2f o;
		o.pos = mul( UNITY_MATRIX_MVP, v.vertex );
		o.screenPos = ComputeScreenPos(o.pos);
		o.uv =  v.texcoord.xy;	
		return o;
	}
	
	half4 fragMain ( v2f IN ) : COLOR {

		half4 mainTex = tex2D (_MainTex, IN.uv);
		mainTex *= _Color;
		mainTex *= _Factor;	
		return mainTex;
		
	}
	
	half4 fragDistort ( v2f IN ) : COLOR {

		float2 screenPos = IN.screenPos.xy / IN.screenPos.w;
	       
		// FIXES UPSIDE DOWN
#if SHADER_API_D3D9
		screenPos.y = 1 - screenPos.y;
#endif
		
		float distFalloff = 1.0 / ( IN.screenPos.z * 0.1 );
		
		half4 mainTex = tex2D( _MainTex, IN.uv );
		
		half4 distort1 = tex2D( _DistortTex, IN.uv * _Tiling1.xy + _Time.y * _Tiling1.zw  );
		half4 distort2 = tex2D( _DistortTex, IN.uv * _Tiling2.xy + _Time.y * _Tiling2.zw  );
		half4 distort3 = tex2D( _DistortTex, IN.uv * _Tiling3.xy + _Time.y * _Tiling3.zw  );
		
		half2 distort = ( ( distort1.xy + distort2.yz + distort2.zx ) - 1.5 ) * 0.01 * _Distortion * distFalloff * mainTex.x;
		
		screenPos += distort;
		
		half4 final = tex2D( _GrabTexture, screenPos );
		final.w = mainTex.w;
		
		return final;
	}
	
	ENDCG
	
	Subshader {
	
		Tags {"Queue" = "Transparent" }
	  		  	
		Pass {
			ZTest LEqual
			ZWrite Off
			Blend SrcAlpha One

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertMain
			#pragma fragment fragMain
			ENDCG
		}
		
		GrabPass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
		}		
		
		Pass {
			ZTest LEqual
			ZWrite Off
			Fog { Mode off }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vertMain
			#pragma fragment fragDistort
			ENDCG
		}
		
	}
}