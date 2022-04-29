# Choice Event Bus

* Decision: Choosing an Event Bus to help with inner commmunication for every microservices, in a synchronous manner.

In the context of `Choosing an Event Bus`, <br>
facing `the need to have a fast and streamlined solution for the microservices to communicate with each other`, <br>
we decided for `RabbitMQ`, <br>
and neglected `Apache Pulsar, Apache ActiveMQ and Apache Kafka`, <br>
to achieve `having a solution for inter communication between microservices`, <br>
accepting `that RabbitMQ cannot process a high amount of messages per instance` <br>