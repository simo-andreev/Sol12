#version v 
 
uniform mat4 projectionMatrix; 
uniform mat4 viewMatrix; 
uniform mat4 modelMatrix; 
 
// Shader toy API uniforms. 
uniform float iTime; // shader playback time (in seconds) 
uniform vec3 iResolution; // viewport resolution (in pixels) 
uniform vec4 iMouse; // mouse pixel coords. xy: current, zw: click 
 
layout(location = 0)in vec3 vertPos; 
layout(location = 1)in vec2 uv; 
layout(location = 2)in float tid; 
layout(location = 3)in vec4 color; 
layout(location = 4)in vec3 normal;
 
// Goes to the frag shader.  
out vec2 UV; 
out vec4 vertColor; 
flat out int Tid; 
out vec3 Normal;
out vec3 fragPos;
 
void main() { 
    // Pass to frag.
    UV = uv;
    vertColor = color;
    Tid = int(tid);
    Normal = normal;
    fragPos = vec3(modelMatrix * vec4(vertPos, 1.0));

    // Multiply by projection.
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertPos, 1.0);
}