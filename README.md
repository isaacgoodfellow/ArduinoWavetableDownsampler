# Downsampler

A converter for taking 8/16 bit .wav files and converting them into wavetable header files / c arrays.

Developed for use with my eurorack sampler sketch [here](https://github.com/isaacgoodfellow/MozmoSampleBlaster)

### Usage:

 * Select "Add" to add multiple files to the conversion list
 * Select "Export" to save multiple wavetable headers to a selected directory (not yet implemented)
 * Select "Copy to clipboard" to quick-copy a C array declarator for the chosen item


### Roadmap

 * Batch output header files
 * Better compatibility checking on file input
 * Better error handling for file I/O problems
 * Configuration file for different formats
 * Lowpass filter to avoid artifacts
 * Dithering
 * Processing on a separate thread / threads
 * Better UX
