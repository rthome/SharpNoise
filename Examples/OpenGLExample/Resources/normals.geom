#version 330 core

layout(triangles) in;
layout(line_strip, max_vertices=8) out;

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
	// vertex normals
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

	// face normal
	vec3 p0 = gl_in[0].gl_Position.xyz + vertex[0].normal.xyz * vertex[0].elevation;
	vec3 p1 = gl_in[1].gl_Position.xyz + vertex[1].normal.xyz * vertex[1].elevation;
	vec3 p2 = gl_in[2].gl_Position.xyz + vertex[2].normal.xyz * vertex[2].elevation;

	vec3 u = p0 - p1;
	vec3 v = p2 - p1;

	vec3 n = normalize(cross(v, u));
	vec3 p = (p0 + p1 + p2) / 3.0;

	gl_Position = MVP * vec4(p, 1);
	VertexColor = vec4(1, 1, 0, 1);
	EmitVertex();

	gl_Position = MVP * vec4(p + n * NormalLength, 1);
	VertexColor = vec4(1, 1, 0, 1);
	EmitVertex();
	EndPrimitive();
}
