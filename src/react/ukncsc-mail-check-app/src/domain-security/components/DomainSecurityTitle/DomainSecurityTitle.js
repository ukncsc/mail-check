import React from 'react';
import PropTypes from 'prop-types';
import { Header, Icon, Loader } from 'semantic-ui-react';
import { getTitleIconProps } from './DomainSecurityTitle.helpers';

import './DomainSecurityTitle.css';

const DomainSecurityTitle = ({
  as,
  title,
  loading,
  subtitle,
  failures,
  warnings,
  inconclusives,
  children,
}) => {
  const icon = getTitleIconProps(failures, warnings, inconclusives);

  return (
    <React.Fragment>
      <Header as={as} className="DomainSecurityTitle--header">
        {title}
      </Header>
      <div className="DomainSecurityTitle--icon">
        {loading && <Loader active inline />}
        {icon && (
          <Icon
            {...icon}
            size="big"
            style={{ lineHeight: as === 'h1' ? '0.3' : '0.4' }}
          />
        )}
      </div>
      {subtitle && <Header as="h2">{subtitle}</Header>}
      {children}
    </React.Fragment>
  );
};

DomainSecurityTitle.defaultProps = {
  as: 'h1',
  loading: false,
  subtitle: null,
  failures: [],
  warnings: [],
  inconclusives: [],
  pending: false,
};

DomainSecurityTitle.propTypes = {
  as: PropTypes.oneOf(['h1', 'h2', 'h3']),
  title: PropTypes.string.isRequired,
  loading: PropTypes.bool,
  subtitle: PropTypes.string,
  failures: PropTypes.arrayOf(PropTypes.string),
  warnings: PropTypes.arrayOf(PropTypes.string),
  inconclusives: PropTypes.arrayOf(PropTypes.string),
  pending: PropTypes.bool,
};

export default DomainSecurityTitle;
