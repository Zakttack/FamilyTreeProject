import React from 'react';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import ChooseFamilyNamePage from './pages/ChooseFamilyNamePage';
import FamilyTreeDashboard from './pages/FamilyTreeDashboard';
import FamilyNameProvider from './providers/FamilyNameProvider';
import './App.css';
import FamilyProfilePage from './pages/FamilyProfilePage';
import FamilyElementProvider from './providers/FamilyElementProvider';

const App: React.FC = () => {
  return (
    <FamilyNameProvider>
      <FamilyElementProvider>
        <Router>
          <div>
            <Routes>
              <Route path='/' element={<ChooseFamilyNamePage />}/>
              <Route path='/dashboard' element={<FamilyTreeDashboard/>}/>
              <Route path='/family-profile' element={<FamilyProfilePage/>}/>
            </Routes>
          </div>
        </Router>
      </FamilyElementProvider>
    </FamilyNameProvider>
  )
}

export default App;
