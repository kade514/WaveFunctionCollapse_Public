// Made with Amplify Shader Editor v1.9.1.6
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WorldspaceOcean"
{
	Properties
	{
		_SurfaceNorm("SurfaceNorm", 2D) = "white" {}
		_Color0("Color 0", Color) = (0.1103182,0,1,1)
		_NormScrollSpeed("NormScrollSpeed", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform sampler2D _SurfaceNorm;
		uniform float2 _NormScrollSpeed;
		uniform float4 _Color0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float2 appendResult5 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 panner3 = ( 1.0 * _Time.y * _NormScrollSpeed + appendResult5);
			o.Normal = UnpackNormal( tex2D( _SurfaceNorm, panner3 ) );
			o.Albedo = _Color0.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19106
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;WorldspaceOcean;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ColorNode;2;-580.6667,-198.8333;Inherit;False;Property;_Color0;Color 0;1;0;Create;True;0;0;0;False;0;False;0.1103182,0,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-586.6667,26.16666;Inherit;True;Property;_SurfaceNorm;SurfaceNorm;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;3;-879.6667,40.16666;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.25,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;5;-1117.467,57.60553;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;6;-1371.867,32.72778;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;8;-1161.667,183.1667;Inherit;False;Property;_NormScrollSpeed;NormScrollSpeed;2;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
WireConnection;0;0;2;0
WireConnection;0;1;1;0
WireConnection;1;1;3;0
WireConnection;3;0;5;0
WireConnection;3;2;8;0
WireConnection;5;0;6;1
WireConnection;5;1;6;3
ASEEND*/
//CHKSM=B56801CB71754CFC602D5B5F3C9EC8C0CD17BC03