# vn-case

Olá, me chamo Rodrigo. Tudo bem com você?

Desenvolvi o case separando as atividades em projetos diferentes. <br>
<b>static_page</b> é o projeto que contém as páginas que o usuário interage e também o responsável por enviar os dados à webapi de captura.<br>
<b>api-captura</b> é a webapi responsável por receber os dados e enviá-los à fila do RabbitMQ.<br>
<b>bot-reader</b> é um console app. Nele é feita a leitura da fila do RabbitMQ, bem como a inserção dos dados obtidos nas bases do Couchbase e do SQL Server.<br>
A última ponta do processo é a <b>api-leitura</b>, onde é possível realizar a consulta dos dados salvos no *Couchbase* em 2 endpoints distintos.<br>
<br><br>
Gostaria de dizer que nunca havia utilizado o RabbitMQ e nem o Couchbase, mas achei ambas as tecnologias muito interessantes.<br>
Refatorei uma boa parte do código que criei ao início do projeto, no entanto, sei que não fiz a refatoração em 100% do código.<br>
<br>
<br>
Ah! Toda a infra fiz através do docker. Caso seja necessário alguma informação a mais sobre o mesmo, só me avisar.<br>
Obrigado pela oportunidade!
