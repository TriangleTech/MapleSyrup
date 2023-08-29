#version 330 core
in vec2 TexCoord;
out vec4 color;

uniform sampler2D image;
uniform vec3 imageColor;
uniform float alpha;

void main()
{
    color = vec4(imageColor, alpha) * texture(image, TexCoord);
}
