// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH/FX_SH_Lazer"
{
	Properties
	{
		_FX_TX_Lazer("FX_TX_Lazer", 2D) = "white" {}
		_Color1("Color 1", Color) = (1,0,0,0)
		_TrailSpeed("Trail Speed", Vector) = (1,0,0,0)
		_TillingPaternA("Tilling Patern A", Vector) = (1,0,0,0)
		_TillingPaternA11("Tilling Patern B", Vector) = (1,0,0,0)
		_TrailSpeed1("Trail Speed", Vector) = (1,0,0,0)
		_Float0("Float 0", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color1;
		uniform sampler2D _FX_TX_Lazer;
		uniform float2 _TrailSpeed;
		uniform float2 _TillingPaternA;
		uniform float2 _TrailSpeed1;
		uniform float2 _TillingPaternA11;
		uniform float _Float0;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord3 = i.uv_texcoord * _TillingPaternA;
			float2 panner2 = ( 1.0 * _Time.y * _TrailSpeed + uv_TexCoord3);
			float4 tex2DNode1 = tex2D( _FX_TX_Lazer, panner2 );
			float2 uv_TexCoord24 = i.uv_texcoord * _TillingPaternA11;
			float2 panner25 = ( 1.0 * _Time.y * _TrailSpeed1 + uv_TexCoord24);
			o.Emission = ( ( _Color1 * ( tex2DNode1.r + tex2D( _FX_TX_Lazer, panner25 ).r ) * _Float0 ) + ( tex2DNode1.g * _Color1 ) ).rgb;
			o.Alpha = ( tex2DNode1.g * 2.0 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
873;81;943;586;1593.07;-43.88349;1.106113;True;False
Node;AmplifyShaderEditor.Vector2Node;29;-1248.393,497.7756;Inherit;False;Property;_TillingPaternA11;Tilling Patern B;5;0;Create;False;0;0;False;0;1,0;3,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;28;-1238.182,148.3429;Inherit;False;Property;_TillingPaternA;Tilling Patern A;4;0;Create;True;0;0;False;0;1,0;0,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;13;-993.1351,283.571;Inherit;False;Property;_TrailSpeed;Trail Speed;3;0;Create;True;0;0;False;0;1,0;-1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1031.786,129.6931;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;23;-1018.31,648.4329;Inherit;False;Property;_TrailSpeed1;Trail Speed;6;0;Create;True;0;0;False;0;1,0;-1.5,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-1056.961,494.5549;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;25;-788.9606,502.555;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;2;-763.786,137.6931;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-548.786,110.693;Inherit;True;Property;_FX_TX_Lazer;FX_TX_Lazer;1;0;Create;True;0;0;False;0;None;c9bb0e67642bc604580f836170999a96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;26;-573.9607,475.5548;Inherit;True;Property;_FX_TX_Lazer1;FX_TX_Lazer;1;0;Create;True;0;0;False;0;None;c9bb0e67642bc604580f836170999a96;True;0;False;white;Auto;False;Instance;1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-50.04403,-259.605;Inherit;False;Property;_Color1;Color 1;2;0;Create;True;0;0;False;0;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-98.48291,9.672516;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;227.296,95.60201;Inherit;False;Property;_Float0;Float 0;7;0;Create;True;0;0;False;0;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;178.9967,183.4884;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;455.296,-10.39798;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;654.0264,158.2584;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;174.884,424.6531;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;955.892,141.9283;Float;False;True;2;ASEMaterialInspector;0;0;Unlit;SH/FX_SH_Lazer;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;28;0
WireConnection;24;0;29;0
WireConnection;25;0;24;0
WireConnection;25;2;23;0
WireConnection;2;0;3;0
WireConnection;2;2;13;0
WireConnection;1;1;2;0
WireConnection;26;1;25;0
WireConnection;27;0;1;1
WireConnection;27;1;26;1
WireConnection;20;0;1;2
WireConnection;20;1;11;0
WireConnection;15;0;11;0
WireConnection;15;1;27;0
WireConnection;15;2;17;0
WireConnection;22;0;15;0
WireConnection;22;1;20;0
WireConnection;18;0;1;2
WireConnection;0;2;22;0
WireConnection;0;9;18;0
ASEEND*/
//CHKSM=DCA64000B6888F2CB15CA044AEA353FCA66F2D07