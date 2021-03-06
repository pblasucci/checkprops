checkprops
===

This repository contains the slide deck, accompanying source code, and errata
for a presentation on random-testing on the CLR (.NET or Mono).

To date, this material has been presented as follows:

+ 27 AUG 2015 at Southeast Valley .NET User Group, AZ (@sevdnug)
+ 09 AUG 2015 at Jet.com Lunch-and-Learn, NJ (@jettechnology)

---

### Some notes about the source code

This solution replies on [Paket](http://fsprojects.github.io/Paket/) for
dependency management. So, you may need to issue the following command before
running any of the tests.

```
> .paket\paket.exe install
```

_Additionally, this code has only been tested on Windows 8.1 (64-bit). However,
other platforms should work with little or no modification._

---

###### The slide deck and source code are released under the MIT license. Please see the [LICENSE](https://github.com/pblasucci/checkprops/blob/master/LICENSE.txt) file for further details.
