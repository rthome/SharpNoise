#version 330 core

layout(location = 0) in vec3 vVertex;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in float vElevation;

out vec4 vFragColor;

uniform mat4 MVP;

const float C1 = 0.429043;
const float C2 = 0.511664;
const float C3 = 0.743125;
const float C4 = 0.886227;
const float C5 = 0.247708;
const vec3 L00  = vec3( 0.7870665,  0.9379944,  0.9799986);
const vec3 L1m1 = vec3( 0.4376419,  0.5579443,  0.7024107);
const vec3 L10  = vec3(-0.1020717, -0.1824865, -0.2749662);
const vec3 L11  = vec3( 0.4543814,  0.3750162,  0.1968642);
const vec3 L2m2 = vec3( 0.1841687,  0.1396696,  0.0491580);
const vec3 L2m1 = vec3(-0.1417495, -0.2186370, -0.3132702);
const vec3 L20  = vec3(-0.3890121, -0.4033574, -0.3639718);
const vec3 L21  = vec3( 0.0872238,  0.0744587,  0.0353051);
const vec3 L22  = vec3( 0.6662600,  0.6706794,  0.5246173);

vec4 sh_light(vec3 normal)
{
	vec3 tnorm = normalize(normal);

	vec3 color = C1 * L22 * (tnorm.x * tnorm.x - tnorm.y * tnorm.y) +
						C3 * L20 * tnorm.z * tnorm.z +
						C4 * L00 -
						C5 * L20 +
						2.0 * C1 * L2m2 * tnorm.x * tnorm.y +
						2.0 * C1 * L21  * tnorm.x * tnorm.z +
						2.0 * C1 * L2m1 * tnorm.y * tnorm.z +
						2.0 * C2 * L11  * tnorm.x +
						2.0 * C2 * L1m1 * tnorm.y +
						2.0 * C2 * L10  * tnorm.z;
	return vec4(color, 1.0);
}

void main()
{
	// Scale elevation to [0, 1] range
	float clampedElevation = clamp((vElevation + 1) * 0.5, 0, 1);

	vec4 shLightColor = sh_light(vNormal);
	vec4 elevationColor = mix(vec4(0, 0, 1, 1), vec4(0.25, 1, 1, 1), clampedElevation);
	vFragColor = mix(elevationColor, shLightColor, 0.2);
	gl_Position = MVP * vec4(vVertex.xy, vElevation * 3, 1);
}