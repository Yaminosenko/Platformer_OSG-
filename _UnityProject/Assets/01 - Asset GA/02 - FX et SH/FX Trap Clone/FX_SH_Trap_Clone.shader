// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH/FX_SH_Trap_Clone"
{
	Properties
	{
		_TextureTrapClone("Texture Trap Clone", 2D) = "white" {}
		[HDR]_ColorGate("Color Gate", Color) = (1,0,0,0)
		[HDR]_ColorDatas("Color Datas", Color) = (1,0,0,0)
		_NoiseScale("Noise Scale", Float) = 5
		[Toggle(_SWITCHTRUESTEPINFALSESTEPOUT_ON)] _SwitchTrueStepINFalseStepOut("Switch True = Step IN / False = Step Out", Float) = 0
		_StepValueInMINtoMax("Step Value In (MIN to Max)", Range( 0 , 1)) = 0.6
		_StepValueOutMAXtoMIN("Step Value Out (MAX to MIN)", Range( 0 , 1.2)) = 0.6
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature _SWITCHTRUESTEPINFALSESTEPOUT_ON
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _ColorDatas;
		uniform sampler2D _TextureTrapClone;
		uniform float4 _ColorGate;
		uniform float4 _TextureTrapClone_ST;
		uniform float _StepValueOutMAXtoMIN;
		uniform float _NoiseScale;
		uniform float _StepValueInMINtoMax;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 panner20 = ( 1.0 * _Time.y * float2( 0,1.3 ) + float2( 0,0 ));
			float2 uv_TexCoord21 = i.uv_texcoord * float2( 1,0.9 ) + panner20;
			float2 panner14 = ( 1.0 * _Time.y * float2( 0,-0.76 ) + float2( 0,0 ));
			float2 uv_TexCoord13 = i.uv_texcoord * float2( -0.7,-0.5 ) + panner14;
			float2 panner50 = ( 1.0 * _Time.y * float2( 0,-0.2 ) + float2( 0,0 ));
			float2 uv_TexCoord51 = i.uv_texcoord * float2( 0.8,0.8 ) + panner50;
			float2 uv_TextureTrapClone = i.uv_texcoord * _TextureTrapClone_ST.xy + _TextureTrapClone_ST.zw;
			float4 tex2DNode10 = tex2D( _TextureTrapClone, uv_TextureTrapClone );
			o.Emission = ( ( _ColorDatas * ( tex2D( _TextureTrapClone, uv_TexCoord21 ).r + tex2D( _TextureTrapClone, uv_TexCoord13 ).r ) ) + ( tex2D( _TextureTrapClone, uv_TexCoord51 ).g * 0.02 ) + ( _ColorGate * tex2DNode10.b ) ).rgb;
			float2 panner4 = ( 1.0 * _Time.y * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord2 = i.uv_texcoord + panner4;
			float simplePerlin2D1 = snoise( uv_TexCoord2*_NoiseScale );
			simplePerlin2D1 = simplePerlin2D1*0.5 + 0.5;
			float smoothstepResult44 = smoothstep( 0.0 , (0.0 + (_StepValueOutMAXtoMIN - 0.0) * (0.5 - 0.0) / (1.0 - 0.0)) , ( pow( ( 1.0 - i.uv_texcoord.y ) , 2.0 ) * simplePerlin2D1 ));
			float smoothstepResult65 = smoothstep( 0.0 , (0.0 + (_StepValueInMINtoMax - 0.0) * (1.5 - 0.0) / (0.6 - 0.0)) , ( simplePerlin2D1 * pow( i.uv_texcoord.y , 2.0 ) ));
			#ifdef _SWITCHTRUESTEPINFALSESTEPOUT_ON
				float staticSwitch54 = ( tex2DNode10.b * smoothstepResult65 );
			#else
				float staticSwitch54 = ( tex2DNode10.b * smoothstepResult44 );
			#endif
			o.Alpha = staticSwitch54;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
168;101;1920;1013;1537.052;-172.0957;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-1207.723,117.6017;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;4;-1310.401,379.8752;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;27;-983.1395,164.3276;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1125.202,382.5468;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-1069.401,552.8753;Inherit;False;Property;_NoiseScale;Noise Scale;4;0;Create;True;0;0;False;0;5;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;56;-1221.196,816.5121;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;20;-1026.003,-1492.305;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,1.3;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;14;-1031.064,-1258.479;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.76;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-646.3388,1106.735;Inherit;False;Property;_StepValueInMINtoMax;Step Value In (MIN to Max);6;0;Create;True;0;0;False;0;0.6;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;50;-1038.59,-802.2994;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-839.3972,-1489.633;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,0.9;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-581.0735,454.6639;Inherit;False;Property;_StepValueOutMAXtoMIN;Step Value Out (MAX to MIN);7;0;Create;True;0;0;False;0;0.6;1.2;0;1.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-887.2029,379.5468;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;100,1000;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-845.8651,-1255.808;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;-0.7,-0.5;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;62;-833.0733,863.238;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;29;-819.6,164.3276;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;12;-624.6507,-1284.444;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;aeafe206e543b8141a2c5a61a0db15fc;aeafe206e543b8141a2c5a61a0db15fc;True;0;False;white;Auto;False;Instance;22;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;64;-287.5117,1103.689;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.6;False;3;FLOAT;0;False;4;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-482.3635,167.7047;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-853.3915,-799.6284;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.8,0.8;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;47;-252.2465,464.796;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;-624.0024,-1518.269;Inherit;True;Property;_TextureTrapClone;Texture Trap Clone;0;0;Create;True;0;0;False;0;aeafe206e543b8141a2c5a61a0db15fc;aeafe206e543b8141a2c5a61a0db15fc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-495.8369,866.6151;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;65;-100.3427,871.2086;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.26;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;-248.4852,-1344.474;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;48;-264.5312,-1543.07;Inherit;False;Property;_ColorDatas;Color Datas;2;1;[HDR];Create;True;0;0;False;0;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;19;-554.7378,-437.8809;Inherit;False;Property;_ColorGate;Color Gate;1;1;[HDR];Create;True;0;0;False;0;1,0,0,0;4.759381,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;10;-593.6292,-175.3076;Inherit;True;Property;_FX_TX_Trap_Clone;FX_TX_Trap_Clone;0;0;Create;True;0;0;False;0;aeafe206e543b8141a2c5a61a0db15fc;aeafe206e543b8141a2c5a61a0db15fc;True;0;False;white;Auto;False;Instance;22;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;52;-633.1771,-828.2644;Inherit;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;0;0;False;0;aeafe206e543b8141a2c5a61a0db15fc;aeafe206e543b8141a2c5a61a0db15fc;True;0;False;white;Auto;False;Instance;22;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;44;-86.86929,172.2982;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.26;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;191.4332,861.1905;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;1.974112,-115.7457;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;170.0395,162.2801;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-15.21894,-1366.717;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-227.7937,-774.1607;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.02;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;163.6031,-800.4067;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;54;458.0294,157.2906;Inherit;False;Property;_SwitchTrueStepINFalseStepOut;Switch True = Step IN / False = Step Out;5;0;Create;True;0;0;False;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;738.7228,-837.9027;Float;False;True;2;ASEMaterialInspector;0;0;Unlit;SH/FX_SH_Trap_Clone;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;3;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;0;24;2
WireConnection;2;1;4;0
WireConnection;21;1;20;0
WireConnection;1;0;2;0
WireConnection;1;1;5;0
WireConnection;13;1;14;0
WireConnection;62;0;56;2
WireConnection;29;0;27;0
WireConnection;12;1;13;0
WireConnection;64;0;61;0
WireConnection;30;0;29;0
WireConnection;30;1;1;0
WireConnection;51;1;50;0
WireConnection;47;0;6;0
WireConnection;22;1;21;0
WireConnection;63;0;1;0
WireConnection;63;1;62;0
WireConnection;65;0;63;0
WireConnection;65;2;64;0
WireConnection;23;0;22;1
WireConnection;23;1;12;1
WireConnection;52;1;51;0
WireConnection;44;0;30;0
WireConnection;44;2;47;0
WireConnection;66;0;10;3
WireConnection;66;1;65;0
WireConnection;18;0;19;0
WireConnection;18;1;10;3
WireConnection;11;0;10;3
WireConnection;11;1;44;0
WireConnection;15;0;48;0
WireConnection;15;1;23;0
WireConnection;53;0;52;2
WireConnection;17;0;15;0
WireConnection;17;1;53;0
WireConnection;17;2;18;0
WireConnection;54;1;11;0
WireConnection;54;0;66;0
WireConnection;0;2;17;0
WireConnection;0;9;54;0
ASEEND*/
//CHKSM=00E6F0EB5ED560C1613FCAA0D571FCB2D2C03AEE