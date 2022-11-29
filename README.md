# CircuitBreakerTeste
Projeto utilizado para testar o pacote de resiliência Polly.
A ideia é utilizar o CircuitBreaker para caso o banco esteja fora do ar, fazendo com que todos os dados inseridos sejam serializados e armazenados no SQLite quando estiver no estado "Open". Quando entrar no estado "HalfOpen" ele deverá fazer a conexão com o banco original e transferir todos os dados armazenados no SQLite.
