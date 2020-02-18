// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH/SH_ShadingPBRGlobal"
{
	Properties
	{
		_SM_TX_GlobalTextures("SM_TX_GlobalTextures", 2D) = "white" {}
		_AlbedoBoost("Albedo Boost", Float) = 2
		_Desaturationpourcentage("Desaturation pourcentage", Float) = 0
		_Rougness("Rougness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _AlbedoBoost;
		uniform sampler2D _SM_TX_GlobalTextures;
		uniform float4 _SM_TX_GlobalTextures_ST;
		uniform float _Desaturationpourcentage;
		uniform float _Rougness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_SM_TX_GlobalTextures = i.uv_texcoord * _SM_TX_GlobalTextures_ST.xy + _SM_TX_GlobalTextures_ST.zw;
			float3 desaturateInitialColor4 = ( _AlbedoBoost * tex2D( _SM_TX_GlobalTextures, uv_SM_TX_GlobalTextures ) ).rgb;
			float desaturateDot4 = dot( desaturateInitialColor4, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar4 = lerp( desaturateInitialColor4, desaturateDot4.xxx, _Desaturationpourcentage );
			o.Albedo = desaturateVar4;
			float temp_output_6_0 = _Rougness;
			o.Metallic = temp_output_6_0;
			o.Smoothness = temp_output_6_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
-15;356;1920;1007;1143;274.5;1;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-506,-4.5;Inherit;True;Property;_SM_TX_GlobalTextures;SM_TX_GlobalTextures;0;0;Create;True;0;0;False;0;4724c7096e61b3441aa7c68a117370f2;4724c7096e61b3441aa7c68a117370f2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-311,-155.5;Inherit;False;Property;_AlbedoBoost;Albedo Boost;1;0;Create;True;0;0;False;0;2;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-142,59.5;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-155,-177.5;Inherit;False;Property;_Desaturationpourcentage;Desaturation pourcentage;2;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;4;92,-26.5;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;6;6,215.5;Inherit;False;Property;_Rougness;Rougness;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;363,7;Float;False;True;2;ASEMaterialInspector;0;0;Standard;SH/SH_ShadingPBRGlobal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;3;0
WireConnection;2;1;1;0
WireConnection;4;0;2;0
WireConnection;4;1;5;0
WireConnection;0;0;4;0
WireConnection;0;3;6;0
WireConnection;0;4;6;0
ASEEND*/
//CHKSM=2366F0ABE67830515E49D01FDB385C78673160E5