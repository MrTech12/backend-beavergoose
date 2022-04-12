# Choose API Gateway

* Decision: choose an API Gateway to help with client communication and centralizing SSL & authentication concerns.

In the context of `Choosing a API Gateway`, <br>
facing `the need have a centralized point for the client to communicate with that support real-time communication`, <br>
we decided for `Ocelot`, <br>
and neglected `Kong Gateway, Tyk API Gateway, KrakenD API Gateway, Express Gateway and Fusio`, <br>
to achieve `having the API Gateway handle the data flow of the real-communication service`, <br>
accepting `that Ocelot does not have rate limiting for real-time protocols`, <br>
because `Ocelot can be managed & packages in the same way as the other services`. <br>