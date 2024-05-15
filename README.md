# WPF.ParticleLife
A particle life playground for C# and WPF.

I should say that I don't understand animation all that well. I also don't understand spacial math too well. So this project, or rather series of projects, is very difficult for me.

I say series of projects because I took the Winforms/C# version, https://github.com/BlinkSun/ParticleLifeSimulation, and made several iterations just trying to learn while experimenting with code.

### WPF.ParticleLife.Ellipses
This project uses WPF Ellipse objects and a Canvas and utilizes the Canvas.SetLeft and Canvas.SetTop methods. While this worked it was not very performant. 

### WPF.ParticleLife.Paths
This project uses WPF Path objects and a Canvas. This project was equal or less performant than the Ellipse counterpart so it was **excluded** from the repo.

### WPF.ParticleLife.StreamGeometry
This project used StreamGeometry with Paths and a Canvas. This project was less performant than the Ellipse or Path counterparts so it was **excluded** from the repo. This really surprised me as the internet said this was the thing to do for performance rendering in WPF. Turned out to not be true in my experience but maybe I was not doing it right.

### WPF.ParticleLife.Graphics
This project uses a System.Drawing.Graphics and a System.Drawing.Bitmap to do GDI+ drawing. This turned out to be the most performant in terms of rendering so this was the selected rendering mechanism moving forward.

### WPF.ParticleLife.Template
This project uses the Graphics Bitmap combo, however the code has been more organized and is well laid out in my opinion. The **ONLY** thing not coded is the particle update code. Look for empty todo!

### WPF.ParticleLife.Updated
This project uses the template project and is an attempt at coming up with a better UpdateParticlePositions algorithm.

## Notes
The Template and Updated variants have a List<Particle> to so you can do nested for loops but this makes it an N^2 problem. The previous iterations had 4 nested loops so it could only be so performant. These two project brought it down to 2 nested for/foreach loops. 

The thing that will speed this up but have not implemented myself is to use quad trees or the Barnes-Hut algorithm. Google one or both of those to speed this up. Also the other thing to consider doing is using the Vector2 type. This apparently is a SIMD-accelerated type in .NET (provides hardware support for performing an operation on multiple pieces of data in parallel using a single instruction)...again Google it. In my experience in playing with this type (you will have to change particle to have 2 Vector2s instead of 4 double properties) is that it made it slower not faster.

I got to the point of trying to play with micro-optimizations seeing how I could speed this up. Nothing really worked.

I got into Parallel.ForEach to try to speed it up but it started to chew up all cores and not really perform all that much better. I furthered that by using OrderablePartitioner<T> to squeeze even better performance out of the Parallel.ForEach but it still was not that great but made a good improvement. Here I could see about 30 fps with 1000 particles but it was eating up my entire PC, all cores. So not that good considering the computing power it was using.

I have a powerful gaming PC and my PC cannot handle more 400 to 500 particles before it starts to really degrade in performance. At 1000 perticles I see about 7 to 9 frames per second. Not good! So maybe getting into quad trees or something else more efficient is the best answer rather than trying to throw computer cores at it.

## Try It Yourself
Open up the template project (or copy it) and look for the empty todo in WPF.ParticleLife.Template.UniverseRenderer.UpdateParticlePositions(double) and take a crack and coming up with an efficient particle position update algorithm. Take some time and familiarize yourself with the code. It is mostly MVVM driven. Feel free to take a look at WPF.ParticleLife.Updated.UniverseRenderer.UpdateParticlePositions to see my attempts at this.

Change the project as you see fit to get a more performant version. I would love to see forks of the project that come up with something more performant than what I am making...which should not be hard as I am struggling here.
