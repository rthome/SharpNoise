#version 330 core

layout(location = 0) in vec3 vVertex;
layout(location = 1) in vec4 vColor;

smooth out vec4 vSmoothColor;

uniform mat4 MVP;

void main()
{
	vSmoothColor = vColor;
	gl_Position = MVP * vec4(vVertex, 1);
}