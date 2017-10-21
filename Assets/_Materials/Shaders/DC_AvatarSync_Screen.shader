Shader "DigitalCombat/DC_AvatarSync_Screen" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Tint ("Tint", Range(0,1)) = 0
	}
	SubShader 
	{
		Tags { "Queue"="Geometry-1" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard fullforwardshadows alpha
			#pragma target 3.0

			fixed4 _Color;
			float _Tint;

			struct Input 
			{
				fixed4 uv_MainTex;
			};

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_CBUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_CBUFFER_END

			void surf (Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = _Color * _Tint;
				o.Albedo = c.rgb;				
				o.Alpha = _Tint;
			}
		ENDCG

		Pass
		{
			ColorMask 0
			ZWrite On
		}
	}
	FallBack "Diffuse"
}
