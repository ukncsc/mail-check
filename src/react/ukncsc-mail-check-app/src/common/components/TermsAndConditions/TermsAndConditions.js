import React from 'react';
import { Container } from 'semantic-ui-react';

export default () => (
  <Container>
    <h1>Terms of Use</h1>
    <p>
      NCSC is operating Mail Check service, with a set of users. You have agreed
      to be a user, on behalf of your organisation.
    </p>
    <p>
      Mail Check will be developed to help measure and improve the security of
      e-mail. Initially those e-mail services used by the UK public sector.
    </p>
    <p>Mail Check handles two mail classes of data. In summary:</p>
    <ol>
      <li>
        <strong>Online look-ups:</strong> NCSC will query the public Domain Name
        System (DNS) and attempt to connect to e-mail servers and initiate a
        secure communication (although NCSC will not actually send e-mail). The
        information exchanged as part of this communication will tell NCSC
        whether your mail servers support the secure exchange of e-mail using
        Transport Layer Security (TLS). If they do not, Mail Check help you take
        steps to avoid e-mail being sent unencrypted over the internet.
      </li>
      <li>
        <strong>DMARC reporting:</strong> NCSC will work to understand whether
        your domains are implementing anti-spoofing controls, including DMARC.
        If anti-spoofing controls are not implemented well, through Mail Check,
        NCSC aims to advise you on how to correctly configure those controls.
        Depending on how you configure your DMARC records, you can allow the
        NCSC to receive DMARC reports on your domains. These reports include
        statistical data, and can also include redacted copies of spoofed or
        legitimate e-mails, depending on the DMARC configuration and what the
        email service provider sends in their DMARC reports.
      </li>
    </ol>
    <p>
      As Mail Check develops, it is expected to reduce the overall impact of
      fake e-mails being sent in the name of public sector organisations. As
      Mail Check is still developing as a service, neither NCSC nor you as a
      user, are under any obligation to (respectively) maintain or use it,
      should it later be decided that it is not in the interest of either/both
      parties to proceed
    </p>
    <p>
      {`NCSC's purpose in providing Mail Check is to provide advice and assistance
      in relation to e-mail security best practice, but NCSC cannot guarantee
      the completeness, quality, or availability of the service. It will be the
      responsibility of the user to determine what further actions may be
      appropriate to take in light of any information provided by the NCSC. It
      will also be the user's responsibility to ensure that their service
      availability is not adversely impacted by Mail Check. As such, NCSC will
      not be liable in respect of any loss caused or arising from the user's use
      of Mail Check.`}
    </p>
    <p>
      NCSC encourages feedback from users, and this will be used to shape the
      roadmap for the service as it continues to develop. NCSC may also use
      information about service activity in order to support improvement and
      further development.
    </p>
    <h2>Use/handling of data</h2>
    <p>
      {`Use, retention, and handling of data will be in line with relevant
      legislation and NCSC's internal policies. Any data received will only be
      retained for as long as there is an operational reason to do so. In
      addition, NCSC intends (subject to appropriate arraignments being in place
      to protect the handling and use of confidential information and personal
      data) to share data received from users through operation of the Mail
      Check service with selected academic partners, in order for NCSC to
      further improve and develop the service.`}
    </p>
    <p>
      Any information supplied by the NCSC to public sector organisations will
      be handled in confidence and not further disclosed by the public sector
      organisation, save where permitted by the NCSC, or where necessary in
      accordance with any relevant legislation.
    </p>
    <p>The user also recognises that:</p>
    <ol>
      <li>
        Information belonging to the NCSC or GCHQ may relate to security or
        intelligence for the purposes of section 1 of the Official Secrets Act
        1989; and
      </li>
      <li>
        The NCSC and GCHQ are exempt from the disclosure provisions of the
        Freedom of Information Act 2000 (FOIA). This exemption extends to ant
        information disclosed by the NCSC to the user in relation to Mail Check.
        Should the user receive a FOIA disclosure request that relates to any
        information related to Mail Check, it will inform the NCSC as soon as
        reasonably practicable and will comply with any reasonable instructions
        of the NCSC for responding to such a request.
      </li>
    </ol>
    <p>
      Additional details on the specific operation of Mail Check, including any
      supplementary terms of use, will be made available as required.
    </p>
  </Container>
);
