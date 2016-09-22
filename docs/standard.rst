..  _Common headers:
..  _standard headers:

Standard request and response headers
==========================================

..  get:: /api/
     :noindex:

    :reqheader Authorization:
        The **string** value ``OAuth2 <token>`` where ``<token>`` is the :term:`access token` for the request.

    :reqheader Accept:
        This header should be set to a value appropriate to the type of data accepted by client. If not specified, default will be ``application/json``

    :resheader Content-Type:
        This header should be set to a value appropriate to the type of data being included in the response. For
        example, when responding to a request with JSON-encoded data in the response body, the
        :http:header:`Content-Type` header should be set to ``application/json``.