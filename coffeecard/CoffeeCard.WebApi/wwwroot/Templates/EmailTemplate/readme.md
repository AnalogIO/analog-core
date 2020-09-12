##Why use mjml for generating the html instead of just creating the html manually?
Different email clients tend to render html and css in different ways, which makes it difficult to create an email that will render the same way across clients. While it can be done, a lot of care needs to be taken when doing so manually. 

Mjml provides several components that should be easy enough to understand for anyone used to html. These components, while only being one line of code in mjml, will generate however much boilerplate html that is required to makje it compatible with various email clients.
    
##Generating the html from mjml
This can be done, either using a plugin for one of the supported editors where mjml is included, or by using the mjml cli tool. Visual studio code is one editor that supports generating the html. 

Currently the html is being commit to our repository since we don't have a pipeline to deal with this yet. 

####Generating all emails
If you already have mjml added to your system path you can run the shell script included in the folder, ```generateEmails.sh```, in order to generate the html for all emails.

####Generating a specific email for debugging
running the followiung command will generate the html in a human readable format, unlike the shell script provided in the folder 

````mjml input.mjml -o output.mjml````

##Editing the mjml
The complete documentation for mjml can be found here https://mjml.io/documentation/ which details what argument can be used for the different components etc.

You can use your editor of choice from the supported list found here: https://mjml.io/documentation/#applications-and-plugins