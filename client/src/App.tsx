import React from 'react';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import ChooseFamilyNamePage from './pages/ChooseFamilyNamePage';
import FamilyTreeDashboard from './pages/FamilyTreeDashboard';
import FamilyNameProvider from './providers/FamilyNameProvider';
import './App.css';
import FamilyRepresentionElementProvider from './providers/FamilyRepresentionElementProvider';
import FamilyProfilePage from './pages/FamilyProfilePage';

const App: React.FC = () => {
  return (
    <FamilyNameProvider>
      <FamilyRepresentionElementProvider>
        <Router>
          <div>
            <Routes>
              <Route path='/' element={<ChooseFamilyNamePage />}/>
              <Route path='/dashboard' element={<FamilyTreeDashboard/>}/>
              <Route path='/family-profile' element={<FamilyProfilePage/>}/>
            </Routes>
          </div>
        </Router>
      </FamilyRepresentionElementProvider>
    </FamilyNameProvider>
  )
}

export default App;
