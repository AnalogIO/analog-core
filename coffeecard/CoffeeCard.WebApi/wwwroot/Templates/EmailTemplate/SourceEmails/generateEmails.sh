#!/bin/sh
for FILE in *.mjml

do
 if [ $FILE == 'footer.mjml' ] || [ $FILE == 'style.mjml'  ]
 then
  echo "Not generating html for footer or style"
 else 
  echo 'Generating html for '$FILE
  base=${FILE%.mjml}
  mjml $FILE --config.beautify false --config.minify true -o ../GeneratedEmails/$base.html
 fi

done