import React from 'react';
import { Divider, Header } from 'semantic-ui-react';

import './DomainSecurityTlsAdvice.css';

const DomainSecurityTlsAdvice = () => (
  <React.Fragment>
    <Header as="h1">TLS Advice</Header>
    <p>
      Transport Layer Security (TLS) is a protocol to allow the encrypted
      exchange of traffic. The client requests the start of a TLS session which
      starts a handshake between the client and server. During the handshake a
      cipher suite is selected, which dictates which cryptographic algorithms
      are used, and any necessary key material is exchanged. It is necessary to
      make sure the server has TLS configured properly to make sure the most
      secure connection possible is set up between the client and server.
    </p>
    <Header as="h2">Protocols</Header>
    <p>
      TLS 1.2 is currently the most modern TLS protocol and should be supported
      by all mail servers. TLS 1.1 and TLS 1.0 are older protocols which should
      also be supported for interoperability. SSL 3.0 is an old protocol which
      is now insecure and should not be supported.
    </p>
    <Header as="h2">Secure cipher suites</Header>
    <p>
      The client offers the mail server a list of cipher suites it can support.
      The Mail Server should have a list of secure cipher suites in the order
      which they are willing to accept them.
    </p>
    <ul>
      <li>
        Key exchange methods which have Perfect Forward Secrecy (PFS), such as
        ECDHE and DHE, should be accepted other those which don’t.
      </li>
      <li>
        Cipher suites which use SHA-2 should be accepted over those using SHA-1.
      </li>
      <li>
        If available it is recommended that AES with a key size of 128 or 256 is
        selected for encryption.
      </li>
      <li>
        The Mail Server should pick it’s preferred cipher suite from the list
        that is offered, not the first member of the list that it accepts.
      </li>
    </ul>
    <p>Cipher suites come in the following format:</p>
    <p className="DomainSecurityTlsAdvice--preformatted DomainSecurityTlsAdvice--cipher_format">
      {
        'TLS_<key_exchange_method>_<cert_signing_method?>_WITH_<encryption_algorithm>_<key_size>_<algorithm_mode?>_<MAC>'
      }
    </p>
    <p>
      Attributes with a {'"?"'} are optional. In the cases where a{' '}
      <span className="DomainSecurityTlsAdvice--preformatted">
        cert_signing_method
      </span>{' '}
      is not specified, it is the same as the{' '}
      <span className="DomainSecurityTlsAdvice--preformatted">
        key_exchange_method
      </span>
      .
    </p>
    <p>
      Below is an example list of secure cipher suites in the order that they
      are to be accepted.
    </p>
    <ul className="DomainSecurityTlsAdvice--preformatted">
      <li>TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384</li>
      <li>TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256</li>
      <li>TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384</li>
      <li>TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256</li>
      <li>TLS_DHE_RSA_WITH_AES_256_GCM_SHA384</li>
      <li>TLS_DHE_RSA_WITH_AES_128_GCM_SHA256</li>
      <li>TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384</li>
      <li>TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256</li>
      <li>TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384</li>
      <li>TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256</li>
      <li>TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA</li>
      <li>TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA</li>
      <li>TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA</li>
      <li>TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA</li>
      <li>TLS_DHE_RSA_WITH_AES_256_CBC_SHA</li>
      <li>TLS_DHE_RSA_WITH_AES_128_CBC_SHA</li>
      <li>TLS_RSA_WITH_AES_256_GCM_SHA384</li>
      <li>TLS_RSA_WITH_AES_128_GCM_SHA256</li>
      <li>TLS_RSA_WITH_AES_256_CBC_SHA256</li>
      <li>TLS_RSA_WITH_AES_128_CBC_SHA256</li>
      <li>TLS_RSA_WITH_AES_256_CBC_SHA</li>
      <li>TLS_RSA_WITH_AES_128_CBC_SHA</li>
      <li>TLS_RSA_WITH_3DES_EDE_CBC_SHA</li>
      <li>TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA</li>
    </ul>
    <Header as="h2">Insecure cipher suites</Header>
    <p>Some cipher suites are insecure and should not be accepted.</p>
    <ul>
      <li>Cipher suites using MD5 as the MAC are insecure.</li>
      <li>
        Cipher suites with NULL for any part of the cipher suite are insecure.
      </li>
      <li>
        Cipher suites with EXPORT for any part of the cipher suite are insecure.
      </li>
      <li>Cipher suites using DES (different from 3DES) are insecure.</li>
    </ul>
    <p>
      Below is a list of insecure cipher suites. Note this is not necessarily
      comprehensive.
    </p>
    <ul className="DomainSecurityTlsAdvice--preformatted">
      <li>RSA_WITH_RC4_128_MD5</li>
      <li>NULL_WITH_NULL_NULL</li>
      <li>RSA_WITH_NULL_MD5</li>
      <li>RSA_WITH_NULL_SHA</li>
      <li>RSA_EXPORT_WITH_RC4_40_MD5</li>
      <li>RSA_EXPORT_WITH_RC2_CBC_40_MD5</li>
      <li>RSA_EXPORT_WITH_DES40_CBC_SHA</li>
      <li>RSA_WITH_DES_CBC_SHA</li>
      <li>DH_DSS_EXPORT_WITH_DES40_CBC_SHA</li>
      <li>DH_DSS_WITH_DES_CBC_SHA</li>
      <li>DH_RSA_EXPORT_WITH_DES40_CBC_SHA</li>
      <li>DH_RSA_WITH_DES_CBC_SHA</li>
      <li>DHE_DSS_EXPORT_WITH_DES40_CBC_SHA</li>
      <li>DHE_DSS_WITH_DES_CBC_SHA</li>
      <li>DHE_RSA_EXPORT_WITH_DES40_CBC_SHA</li>
      <li>DHE_RSA_WITH_DES_CBC_SHA</li>
    </ul>
    <Header as="h2">Extensions</Header>
    <p>
      The client also supplies extensions. The supported groups extension
      contains the list of Elliptic Curves that are supported by the client. If
      an Elliptic Curve Cipher Suite is selected then the Mail Server should
      pick an Elliptic Curve from this group. The size of the Elliptic Curve
      should be greater than or equal to 256 bits. If available we recommend one
      of the curves specified in RFC5480 shown below:
    </p>
    <ul className="DomainSecurityTlsAdvice--preformatted">
      <li>Sect571r1</li>
      <li>Sect571k1</li>
      <li>Secp521r1</li>
      <li>Sect409k1</li>
      <li>Sect409r1</li>
      <li>Secp384r1</li>
      <li>Sect283k1</li>
      <li>Sect283r1</li>
      <li>Secp256r1</li>
    </ul>
    <p>
      If the client supports RFC7919 then the supported groups extension also
      contains the list of supported Diffie-Hellman. If available we recommend
      selecting one of the groups:
    </p>
    <ul className="DomainSecurityTlsAdvice--preformatted">
      <li>ffdhe2048</li>
      <li>ffdh23072</li>
      <li>ffdhe4096</li>
    </ul>
    <p>
      If no groups are offered, but a Diffie-Hellman Cipher Suite is selected, a
      group of size greater than or equal to 2048 bits should be used.
    </p>
    <Divider hidden />
  </React.Fragment>
);

export default DomainSecurityTlsAdvice;
