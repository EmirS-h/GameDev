#version 330

in vec2 fragTexCoord;
in vec4 fragColor;

uniform sampler2D texture0;
uniform vec4 colDiffuse;

out vec4 finalColor;

uniform vec2 u_texture_size;
uniform vec3 u_outline_color;

const float RADIUS = 3.0;
const float BLOOM_INTENSITY = 1.0;

void main()
{
    vec4 center_pixel = texture(texture0, fragTexCoord);
    
    if (center_pixel.a > 0.0) {
        finalColor = center_pixel;
        return;
    }

    vec2 pixel_size = 1.0 / u_texture_size;
    float max_alpha = 0.0;

    for (float x = -RADIUS; x <= RADIUS; x += 1.0) {
        for (float y = -RADIUS; y <= RADIUS; y += 1.0) {
            
            if (x*x + y*y > RADIUS*RADIUS) continue;

            vec2 offset = vec2(x, y) * pixel_size;
            float neighbor_a = texture(texture0, fragTexCoord + offset).a;

            if (neighbor_a > 0.0) {
                float dist = length(vec2(x, y));
                float current_alpha = 0.0;

                if (dist <= 1.5f) {
                    // Layer A: Sharp Outline
                    current_alpha = 1.0f;
                } else {
                    // Layer B: Soft Bloom
                    float falloff = 1.0 - (dist / RADIUS);
                    current_alpha = falloff * BLOOM_INTENSITY;
                }

                max_alpha = max(max_alpha, current_alpha);
            }
        }
    }

    if (max_alpha <= 0.0) discard;
    
    finalColor = vec4(u_outline_color, max_alpha);
}