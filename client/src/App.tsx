import React, { useContext } from 'react';
import {BrowserRouter as Router, Routes, Route} from 'react-router-dom';
import ChooseFamilyNamePage from './pages/ChooseFamilyNamePage';
import FamilyNameProvider from './providers/FamilyNameProvider';
import './App.css';
import FamilyProfilePage from './pages/FamilyProfilePage';
import FamilyElementProvider from './providers/FamilyElementProvider';
import FamilyTreePage from './pages/FamilyTreePage';
import TitleProvider from './providers/TitleProvider';
import FamilySubTreePage from './pages/FamilySubTreePage';
import CriticalAttributeProvider from './providers/CriticalAttributeProvider';
import CriticalAttributeManager from './components/CriticalAttributeManager';
import { CriticalAttribute } from './Enums';
import CriticalAttributeContext from './contexts/CriticalAttributeContext';
import { isSuccess } from './Utils';

const App: React.FC = () => {
  const {criticalAttributeResponse} = useContext(CriticalAttributeContext);
  return (
    <CriticalAttributeProvider>
      <TitleProvider>
        <CriticalAttributeManager criticalAttribute={CriticalAttribute.Title} />
        {isSuccess(criticalAttributeResponse) && (
          <FamilyNameProvider>
            <CriticalAttributeManager criticalAttribute={CriticalAttribute.FamilyName} />
            {isSuccess(criticalAttributeResponse) && (
              <FamilyElementProvider>
                <CriticalAttributeManager criticalAttribute={CriticalAttribute.SelectedFamily} />
                {isSuccess(criticalAttributeResponse) && (
                  <Router>
                    <Routes>
                      <Route path='/' element={<ChooseFamilyNamePage />}/>
                      <Route path='/family-tree' element={<FamilyTreePage/>}/>
                      <Route path='/family-profile' element={<FamilyProfilePage/>}/>
                      <Route path='/sub-tree' element={<FamilySubTreePage />}/>
                    </Routes>
                  </Router>
                )}
            </FamilyElementProvider>
            )}
        </FamilyNameProvider>
        )}
      </TitleProvider>
    </CriticalAttributeProvider>
  )
}

export default App;
