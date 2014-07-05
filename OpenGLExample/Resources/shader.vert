#version 330 core

layout(location = 0) in vec3 vVertex;
layout(location = 1) in float vElevation;

out vec4 vFragColor;

uniform mat4 MVP;

void main()
{
	// Scale elevation to [0, 1] range
	float clampedElevation = clamp((vElevation + 1) * 0.5, 0, 1);

	vFragColor = vec4(vec3(clampedElevation), 1);
	gl_Position = MVP * vec4(vVertex.xy, vVertex.z + vElevation, 1);
}