import React from 'react';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import ChooseFamilyNamePage from './pages/ChooseFamilyNamePage';
import './App.css';
import FamilyProfilePage from './pages/FamilyProfilePage';
import FamilyTreePage from './pages/FamilyTreePage';
import FamilySubTreePage from './pages/FamilySubTreePage';
import FamilyNameProvider from './providers/FamilyNameProvider';
import TitleProvider from './providers/TitleProvider';

const App: React.FC = () => {
  return (
    <FamilyNameProvider>
      <TitleProvider>
        <Router>
          <Routes>
            <Route path='/' element={<ChooseFamilyNamePage />}/>
            <Route path='/family-tree' element={<FamilyTreePage/>}/>
            <Route path='/family-profile' element={<FamilyProfilePage/>}/>
            <Route path='/sub-tree' element={<FamilySubTreePage />}/>
          </Routes>
        </Router>
      </TitleProvider>
    </FamilyNameProvider>
  )
}

export default App;
