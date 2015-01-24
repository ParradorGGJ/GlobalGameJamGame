Shader "Custom/Triplanar_Shader" 
{
	Properties 
	{
		_Tint ( "Tint", Color ) = ( 1, 1, 1, 1 )

		_DiffuseTexture ("Texture", 2D) = "white" {}
		
		_UVScale ("Scale", Range(0.1, 10.0)) = 1.0
		
		_UVXCoord ("X UV Coordinate", Float) = 0.0
		_UVYCoord ("Y UV Coordinate", Float) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		Alphatest Greater 0.1

		CGPROGRAM
		#pragma surface surf Lambert

		float4 _Tint;
		sampler2D _DiffuseTexture;
		float _UVScale;
		float _UVXCoord;
		float _UVYCoord;

		struct Input 
		{
			float3 worldPos;
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half2 uvX = IN.worldPos.zy / _UVScale;
			half2 uvY = IN.worldPos.xz / _UVScale;
			half2 uvZ = IN.worldPos.xy / _UVScale;
			
			half3 xDiffuse = tex2D(_DiffuseTexture, uvX);
			half3 yDiffuse = tex2D(_DiffuseTexture, uvY);
			half3 zDiffuse = tex2D(_DiffuseTexture, uvZ);
			
			half3 blendWeight = abs(IN.worldNormal);
			
			blendWeight = blendWeight / (blendWeight.x + blendWeight.y + blendWeight.z );
			
			o.Albedo = xDiffuse * blendWeight.x + yDiffuse * blendWeight.y + zDiffuse * blendWeight.z;

			o.Albedo = o.Albedo * _Tint;
		}
		ENDCG
	} 
}


