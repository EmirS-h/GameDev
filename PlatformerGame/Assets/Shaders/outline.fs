#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 colDiffuse;

// Output fragment color
out vec4 finalColor;

// Custom Uniforms
uniform vec2 u_texture_size;
uniform vec3 u_outline_color;

// --- Constants from your original code ---
const float RADIUS = 3.0;           // [cite: 25]
const float BLOOM_INTENSITY = 1.0;  // [cite: 26]

void main()
{
    // 1. Get the pixel at the current coordinate
    vec4 center_pixel = texture(texture0, fragTexCoord);
    
    // If this pixel is not transparent, just draw the texture normally
    if (center_pixel.a > 0.0) {
        finalColor = center_pixel;
        return;
    }

    // 2. Setup for neighbor scanning
    vec2 pixel_size = 1.0 / u_texture_size;
    float max_alpha = 0.0;

    // 3. Scan Loop 
    for (float x = -RADIUS; x <= RADIUS; x += 1.0) {
        for (float y = -RADIUS; y <= RADIUS; y += 1.0) {
            
            // Skip corners to make a circle shape
            if (x*x + y*y > RADIUS*RADIUS) continue;

            vec2 offset = vec2(x, y) * pixel_size;
            float neighbor_a = texture(texture0, fragTexCoord + offset).a;

            if (neighbor_a > 0.0) {
                float dist = length(vec2(x, y));
                float current_alpha = 0.0;

                // --- YOUR HYBRID LOGIC [cite: 33-35] ---
                if (dist <= 1.5) {
                    // Layer A: Sharp Outline
                    current_alpha = 1.0;
                } else {
                    // Layer B: Soft Bloom
                    float falloff = 1.0 - (dist / RADIUS);
                    current_alpha = falloff * BLOOM_INTENSITY;
                }

                max_alpha = max(max_alpha, current_alpha);
            }
        }
    }

    // 4. Output
    if (max_alpha <= 0.0) discard; // Optimization
    
    finalColor = vec4(u_outline_color, max_alpha);
}