import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from './Components/Routes/AppRoutes';
import Navbar from './Components/PostComments/DashBoard';

function App() {
  return (
    <Router>
      <Navbar/>
      <AppRoutes />
    </Router>
  );
}

export default App;
