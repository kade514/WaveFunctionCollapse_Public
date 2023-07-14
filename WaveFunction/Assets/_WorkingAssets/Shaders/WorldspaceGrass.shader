// Made with Amplify Shader Editor v1.9.1.6
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WorldspaceGrass"
{
	Properties
	{
		_Main("Main", 2D) = "white" {}
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
			float3 worldPos;
		};

		uniform sampler2D _Main;


		void StochasticTiling( float2 UV, out float2 UV1, out float2 UV2, out float2 UV3, out float W1, out float W2, out float W3 )
		{
			float2 vertex1, vertex2, vertex3;
			// Scaling of the input
			float2 uv = UV * 3.464; // 2 * sqrt (3)
			// Skew input space into simplex triangle grid
			const float2x2 gridToSkewedGrid = float2x2( 1.0, 0.0, -0.57735027, 1.15470054 );
			float2 skewedCoord = mul( gridToSkewedGrid, uv );
			// Compute local triangle vertex IDs and local barycentric coordinates
			int2 baseId = int2( floor( skewedCoord ) );
			float3 temp = float3( frac( skewedCoord ), 0 );
			temp.z = 1.0 - temp.x - temp.y;
			if ( temp.z > 0.0 )
			{
				W1 = temp.z;
				W2 = temp.y;
				W3 = temp.x;
				vertex1 = baseId;
				vertex2 = baseId + int2( 0, 1 );
				vertex3 = baseId + int2( 1, 0 );
			}
			else
			{
				W1 = -temp.z;
				W2 = 1.0 - temp.y;
				W3 = 1.0 - temp.x;
				vertex1 = baseId + int2( 1, 1 );
				vertex2 = baseId + int2( 1, 0 );
				vertex3 = baseId + int2( 0, 1 );
			}
			UV1 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex1 ) ) * 43758.5453 );
			UV2 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex2 ) ) * 43758.5453 );
			UV3 = UV + frac( sin( mul( float2x2( 127.1, 311.7, 269.5, 183.3 ), vertex3 ) ) * 43758.5453 );
			return;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float localStochasticTiling2_g1 = ( 0.0 );
			float3 ase_worldPos = i.worldPos;
			float2 appendResult2 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 Input_UV145_g1 = appendResult2;
			float2 UV2_g1 = Input_UV145_g1;
			float2 UV12_g1 = float2( 0,0 );
			float2 UV22_g1 = float2( 0,0 );
			float2 UV32_g1 = float2( 0,0 );
			float W12_g1 = 0.0;
			float W22_g1 = 0.0;
			float W32_g1 = 0.0;
			StochasticTiling( UV2_g1 , UV12_g1 , UV22_g1 , UV32_g1 , W12_g1 , W22_g1 , W32_g1 );
			float2 temp_output_10_0_g1 = ddx( Input_UV145_g1 );
			float2 temp_output_12_0_g1 = ddy( Input_UV145_g1 );
			float4 Output_2D293_g1 = ( ( tex2D( _Main, UV12_g1, temp_output_10_0_g1, temp_output_12_0_g1 ) * W12_g1 ) + ( tex2D( _Main, UV22_g1, temp_output_10_0_g1, temp_output_12_0_g1 ) * W22_g1 ) + ( tex2D( _Main, UV32_g1, temp_output_10_0_g1, temp_output_12_0_g1 ) * W32_g1 ) );
			float4 Albedo12 = Output_2D293_g1;
			o.Albedo = Albedo12.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19106
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;276,-113;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;WorldspaceGrass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.FunctionNode;10;-628.6402,-107.8275;Inherit;False;Procedural Sample;-1;;1;f5379ff72769e2b4495e5ce2f004d8d4;2,157,0,315,0;7;82;SAMPLER2D;0;False;158;SAMPLER2DARRAY;0;False;183;FLOAT;0;False;5;FLOAT2;0,0;False;80;FLOAT3;0,0,0;False;104;FLOAT2;1,1;False;74;SAMPLERSTATE;0;False;5;COLOR;0;FLOAT;32;FLOAT;33;FLOAT;34;FLOAT;35
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1091.81,-309.6124;Inherit;True;Property;_Main;Main;0;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.DynamicAppendNode;2;-821.6431,-26.96987;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-1076.043,-51.84761;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-380.7094,-101.6125;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;39.19065,-110.7125;Inherit;False;12;Albedo;1;0;OBJECT;;False;1;COLOR;0
WireConnection;0;0;13;0
WireConnection;10;82;11;0
WireConnection;10;5;2;0
WireConnection;2;0;1;1
WireConnection;2;1;1;3
WireConnection;12;0;10;0
ASEEND*/
//CHKSM=AA96A4B427C67D45320BCF077FAC2A5E5156B5C3