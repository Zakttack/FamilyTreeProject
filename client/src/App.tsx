import React from 'react';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import ChooseFamilyNamePage from './pages/ChooseFamilyNamePage';
import FamilyTreeDashboard from './pages/FamilyTreeDashboard';
import FamilyNameProvider from './providers/FamilyNameProvider';
import './App.css';

const App: React.FC = () => {
  return (
    <FamilyNameProvider>
      <Router>
        <div>
          <Routes>
            <Route path='/' element={<ChooseFamilyNamePage />}/>
            <Route path='/dashboard' element={<FamilyTreeDashboard/>}/>
          </Routes>
        </div>
      </Router>
    </FamilyNameProvider>
  )
}

export default App;
