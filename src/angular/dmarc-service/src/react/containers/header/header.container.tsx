import * as React from 'react';
import { connect } from 'react-redux';
import { Banner } from '../../components';
import { fetchUser } from '../../stores/user';

class Header extends React.Component<any, any> {
    constructor(props) {
        super(props);
        
    }

    componentWillMount () {
        this.props.fetchUser();
      };
    
    render () {
        return <Banner companyLogo="/a/app/img/logo.png" productName="Mail Check" user={this.props.user} />
    }
}

const mapStateToProps = ({ user }) => ({ user });

const mapDispatchToProps = dispatch => ({
    fetchUser: () => dispatch(fetchUser())
});

export default connect(mapStateToProps, mapDispatchToProps)(
  Header
);