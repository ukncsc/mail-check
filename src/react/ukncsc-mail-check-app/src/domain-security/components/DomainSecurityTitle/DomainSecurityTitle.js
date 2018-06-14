import React from 'react';
import PropTypes from 'prop-types';
import { Header, Icon, Loader } from 'semantic-ui-react';
import moment from 'moment';
import { getTitleIconProps } from './DomainSecurityTitle.helpers';

import './DomainSecurityTitle.css';

const DomainSecurityTitle = ({
  loading,
  subtitle,
  failures,
  warnings,
  inconclusives,
  pending,
  lastChecked,
  as,
  children,
}) => {
  const icon = getTitleIconProps(failures, warnings, inconclusives, pending);

  return (
    <React.Fragment>
      <Header as={as} className="DomainSecurityTitle--header">
        {children}
      </Header>
      <div className="DomainSecurityTitle--icon">
        {loading && <Loader active inline />}
        {!loading &&
          icon && (
            <Icon
              {...icon}
              size="big"
              style={{ lineHeight: as === 'h1' ? '0.3' : '0.4' }}
            />
          )}
      </div>
      {subtitle && <Header as="h2">{subtitle}</Header>}
      {lastChecked && <p>Last checked {moment(lastChecked).fromNow()}</p>}
    </React.Fragment>
  );
};

DomainSecurityTitle.defaultProps = {
  loading: false,
  subtitle: null,
  failures: [],
  warnings: [],
  inconclusives: [],
  pending: false,
  lastChecked: null,
  as: 'h1',
};

DomainSecurityTitle.propTypes = {
  loading: PropTypes.bool,
  subtitle: PropTypes.string,
  failures: PropTypes.arrayOf(PropTypes.string),
  warnings: PropTypes.arrayOf(PropTypes.string),
  inconclusives: PropTypes.arrayOf(PropTypes.string),
  pending: PropTypes.bool,
  lastChecked: PropTypes.string,
  as: PropTypes.oneOf(['h1', 'h2']),
};

export default DomainSecurityTitle;
