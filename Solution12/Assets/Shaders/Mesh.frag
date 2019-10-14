#version v 
 
#ifdef GL_ES 
precision highp float; 
#endif 
 
uniform sampler2D textures[16]; 
uniform vec3 iResolution; // viewport resolution (in pixels)
uniform vec3 Sun;
 
// Comes in from the vertex shader. 
in vec2 UV; 
in vec4 vertColor; 
flat in int Tid;
in vec3 Normal;
in vec3 fragPos;

out vec4 fragColor; 
 
//GetTextureColor
 
void main() { 
    fragColor = getTextureColor(Tid, UV) * vertColor;
    vec3 lightDir = normalize(Sun - fragPos);
    float diffusion = max(dot(Normal, lightDir), 0.0);
    fragColor = diffusion * fragColor;

    if (fragColor.a < 0.01)discard;
}