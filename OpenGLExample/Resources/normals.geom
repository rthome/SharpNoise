#version 330 core

layout(triangles) in;
layout(line_strip, max_vertices=6) out;

uniform mat4 MVP;
uniform float NormalLength;

in Vertex
{
	vec4 normal;
	vec4 color;
	float elevation;
} vertex[];

out vec4 VertexColor;

void main()
{
	for(int i = 0; i < gl_in.length(); i++)
	{
		vec3 p = gl_in[i].gl_Position.xyz;
		vec3 n = vertex[i].normal.xyz;

		vec3 ps = p + n * vertex[i].elevation;

		gl_Position = MVP * vec4(ps, 1);
		VertexColor = vertex[i].color;
		EmitVertex();

		gl_Position = MVP * vec4(ps + n * NormalLength, 1);
		VertexColor = vertex[i].color;
		EmitVertex();

		EndPrimitive();
	}
}
