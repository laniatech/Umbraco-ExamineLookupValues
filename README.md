# Umbraco-ExamineLookupValues
Includes names of nodes selected via Multi-Node Treepickers (MNTP) into Examine indexes instead of the node IDs to facilitate Examine searches

By default, Examine indexes tread MNTP property values as a comma delimited list of ids, providing little value to search. With ExamineLookupValues, you can have those comma delimited list of values replaced by a space-delimited list of actual node names.

For example, let's suppose you have a "BlogPost" document type that contains a "categoryPicker" property, which is an MNTP datatype, letting you select "Category" nodes that you maintain somewhere else in your Umbraco installation. Let's also suppose you select two categories for a new blog post, named "Programming" and "SEO" respectively. What goes in the Examine index for your blog post will be a list of IDs for those two categories, like "1049,1055". 

if you have defined a rule (see below) for the "BlogPost" document and the "categoryPicker" property then the Examine contents for the "categoryPicker" property will be replaced with actual category names. So the value will become "Programming SEO". Now, when you search blog posts with Examine for "SEO" or "Programming" your blog post will come up.

This works via a config file named DotSee.Elv.Config (placed in the /config folder). There, you can create rules that define:
- The document type alias to watch for
- The property alias to check

Like this (based on the example above):
```
<rule docTypeAlias="BlogPost" propertyAlias="categoryPicker"/>
```

Two things to note:
- If you change the name of a lookup node, the index values will not be updated for pickers that reference it. In this case, only a full index rebuild will update those nodes.
- Since this is doing a lookup for every node included in the picker, it may slow down your publishing and index rebuild process a bit if you have a large number of nodes and also many nodes selected in pickers. In my experience, this will start to show after you have a significant amount of nodes (10k or more).
