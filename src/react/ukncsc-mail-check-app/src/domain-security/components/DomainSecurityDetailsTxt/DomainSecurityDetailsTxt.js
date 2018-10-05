import React from 'react';
import PropTypes from 'prop-types';
import { Divider, Message } from 'semantic-ui-react';
import moment from 'moment';
import {
  DomainSecurityDetailsMessages,
  DomainSecurityRecord,
  DomainSecurityRecordExplanation,
  DomainSecurityTitle,
} from 'domain-security/components';
import {
  Breadcrumb,
  BreadcrumbItem,
  ShowMoreDropdown,
  MailCheckMessage,
} from 'common/components';
import { DomainSecurityLocationContext } from 'domain-security/context';

const DomainSecurityDetailsTxt = ({
  type,
  pending,
  domainId,
  domainName,
  loading,
  error,
  records,
  failures,
  warnings,
  inconclusives,
  tagsProperty,
  lastChecked,
  inheritedFrom,
  children,
}) => (
  <React.Fragment>
    <Breadcrumb>
      <DomainSecurityLocationContext.Consumer>
        {location => <BreadcrumbItem link={`/${location}/${domainId}`}>{domainName}</BreadcrumbItem>}
      </DomainSecurityLocationContext.Consumer>
      <BreadcrumbItem active>{type}</BreadcrumbItem>
    </Breadcrumb>
    <DomainSecurityTitle
      title={type}
      subtitle={domainName}
      loading={loading}
      error={error}
      failures={failures}
      warnings={warnings}
      inconclusives={inconclusives}
    >
      {lastChecked && <p>Last Checked {moment.utc(lastChecked).local().fromNow()}</p>}
    </DomainSecurityTitle>
    {children}
    {error && <Message error>{error.message}</Message>}
    {records && (
      <React.Fragment>
        {records.map(r => (
          <React.Fragment key={r.index}>
            <DomainSecurityRecord inheritedFrom={inheritedFrom} type={type}>
              {r.record}
            </DomainSecurityRecord>
            <ShowMoreDropdown title="Explain record">
              <DomainSecurityRecordExplanation
                title="Discovered Tags"
                data={r[tagsProperty].filter(t => !t.isImplicit)}
              />
              <DomainSecurityRecordExplanation
                title="Implicit Tags"
                data={r[tagsProperty].filter(t => t.isImplicit)}
              />
            </ShowMoreDropdown>
            <Divider hidden />
          </React.Fragment>
        ))}
        {!loading &&
          !error && (
            <DomainSecurityDetailsMessages
              markdown
              type={type}
              failures={failures}
              warnings={warnings}
              inconclusives={inconclusives}
            />
          )}
      </React.Fragment>
    )}
    {pending && (
      <MailCheckMessage info>
        {`We're currently checking your ${type} configuration. This usually takes less than 15 minutes, so check back soon.`}
      </MailCheckMessage>
    )}
  </React.Fragment>
);

DomainSecurityDetailsTxt.defaultProps = {
  pending: false,
  domainName: null,
  loading: false,
  error: null,
  records: [],
  failures: [],
  warnings: [],
  inconclusives: [],
  tagsProperty: 'tags',
  lastChecked: null,
  inheritedFrom: null,
};

DomainSecurityDetailsTxt.propTypes = {
  domainId: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired,
  pending: PropTypes.bool,
  domainName: PropTypes.string,
  loading: PropTypes.bool,
  error: PropTypes.shape({ message: PropTypes.string }),
  records: PropTypes.arrayOf(PropTypes.shape({ record: PropTypes.string })),
  failures: PropTypes.arrayOf(PropTypes.string),
  warnings: PropTypes.arrayOf(PropTypes.string),
  inconclusives: PropTypes.arrayOf(PropTypes.string),
  tagsProperty: PropTypes.string,
  lastChecked: PropTypes.string,
  inheritedFrom: PropTypes.string,
};

export default DomainSecurityDetailsTxt;
