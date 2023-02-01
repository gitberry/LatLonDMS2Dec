# LatLonDMS2Dec
A simple utility to convert Lat/Lon DMS coordinates to D decimal. 

This can be a simple utility - or with extra parameters become a very handy                                      
tool for outputing consistently formatted GEOGRAPHIC data.                                                       

Original incentive to build was when I had a need to process EXIF data from iPhone photos into geographic map. 

## Elaboration ##

OK - so you're doing some project that ultimately requires a map. 
You are taking photos with you phone
and you've got it configured to store coordinate location so you can automate
the creation of a map - this tool is one part of the automation 
process that allows you to take the output of a 
EXIFextraction tool and insert it into something else - say a KML file
that you import into a mapping utility - say google maps. All of this
is demonstrated in AutomationNotes.txt in the solution.

Alternatively - I could have exported everything to a database - then coded something to export geoJSON or whatever.. this was just one path to an end result.  The method really depends on how much time you want to spend on it - AND - what the nature of the end result is...


### Dependencies

This is a .NET core 3.1 console app.  I tweaked compile to create a single EXE for ease. 

## Authors

Contributors names and contact info

gitberry@northberry.ca - the primary github handle for Clark Rensberry

## Version History

V 1.0 - Jan 25, 2023 - tests out and using it for a particular project.

## License

This project is licensed under the BSD License - copy, use, etc, attribute if you want, no blame accepted, acknowledged or even considered. 

## Acknowledgments

Various acknowledgements within the code where appropriate.
