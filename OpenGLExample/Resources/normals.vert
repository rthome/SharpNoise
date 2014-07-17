#version 330 core

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in float vElevation;

out Vertex
{
	vec4 normal;
	vec4 color;
	float elevation;
} vertex;

void main()
{
	vertex.normal = vec4(vNormal, 1);
	vertex.color = vec4(1, 0, 0, 1);
	vertex.elevation = vElevation;
	gl_Position = vec4(vPosition, 1);
}
