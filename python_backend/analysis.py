"""
Scientific App Python Backend - Analysis Module
Provides regression and statistical analysis for CS109x data analysis
"""

import json
import numpy as np
from typing import List, Dict


def linear_regression(x_data: List[float], y_data: List[float]) -> Dict:
    """
    Performs linear regression: y = a + b*x using least squares
    
    Args:
        x_data: Independent variable values
        y_data: Dependent variable values
    
    Returns:
        Dict with keys: 'intercept', 'slope', 'success'
    """
    try:
        if not x_data or not y_data or len(x_data) != len(y_data):
            return {'success': False, 'error': 'Invalid input data'}
        
        x = np.array(x_data, dtype=float)
        y = np.array(y_data, dtype=float)
        
        n = len(x)
        sum_x = np.sum(x)
        sum_y = np.sum(y)
        sum_xx = np.sum(x * x)
        sum_xy = np.sum(x * y)
        
        denominator = n * sum_xx - sum_x * sum_x
        if abs(denominator) < 1e-10:
            return {'success': False, 'error': 'Singular matrix (zero denominator)'}
        
        slope = (n * sum_xy - sum_x * sum_y) / denominator
        intercept = (sum_y - slope * sum_x) / n
        
        return {
            'success': True,
            'intercept': float(intercept),
            'slope': float(slope)
        }
    except Exception as e:
        return {'success': False, 'error': str(e)}


def polynomial_regression(x_data: List[float], y_data: List[float], degree: int) -> Dict:
    """
    Performs polynomial regression of specified degree
    
    Args:
        x_data: Independent variable values
        y_data: Dependent variable values
        degree: Polynomial degree (1, 2, 3, etc.)
    
    Returns:
        Dict with keys: 'coefficients' (highest to lowest degree), 'success'
    """
    try:
        if not x_data or not y_data or len(x_data) != len(y_data):
            return {'success': False, 'error': 'Invalid input data'}
        
        if degree < 1 or degree > 5:
            return {'success': False, 'error': 'Degree must be 1-5'}
        
        x = np.array(x_data, dtype=float)
        y = np.array(y_data, dtype=float)
        
        # Fit polynomial
        coeffs = np.polyfit(x, y, degree)
        
        return {
            'success': True,
            'coefficients': coeffs.tolist()  # [a_n, a_{n-1}, ..., a_1, a_0]
        }
    except Exception as e:
        return {'success': False, 'error': str(e)}


def calculate_metrics(y_true: List[float], y_pred: List[float], num_params: int) -> Dict:
    """
    Calculate regression metrics: R², RMSE, AIC
    
    Args:
        y_true: Observed values
        y_pred: Predicted values
        num_params: Number of model parameters (for AIC)
    
    Returns:
        Dict with keys: 'r_squared', 'rmse', 'aic', 'success'
    """
    try:
        y_true = np.array(y_true, dtype=float)
        y_pred = np.array(y_pred, dtype=float)
        
        if len(y_true) != len(y_pred):
            return {'success': False, 'error': 'Mismatched array lengths'}
        
        # R-squared
        ss_res = np.sum((y_true - y_pred) ** 2)
        ss_tot = np.sum((y_true - np.mean(y_true)) ** 2)
        r_squared = 1.0 - (ss_res / ss_tot) if ss_tot > 0 else 0.0
        
        # RMSE
        rmse = float(np.sqrt(np.mean((y_true - y_pred) ** 2)))
        
        # AIC
        n = len(y_true)
        aic = n * np.log(ss_res / n) + 2 * num_params
        
        return {
            'success': True,
            'r_squared': float(r_squared),
            'rmse': float(rmse),
            'aic': float(aic)
        }
    except Exception as e:
        return {'success': False, 'error': str(e)}


def evaluate_polynomial(coefficients: List[float], x_values: List[float]) -> Dict:
    """
    Evaluate polynomial at given x values
    
    Args:
        coefficients: Polynomial coefficients [a_n, a_{n-1}, ..., a_0] (highest to lowest)
        x_values: Points to evaluate
    
    Returns:
        Dict with keys: 'predictions', 'success'
    """
    try:
        if not coefficients or not x_values:
            return {'success': False, 'error': 'Empty input'}
        
        x = np.array(x_values, dtype=float)
        coeffs = np.array(coefficients, dtype=float)
        
        # Use numpy.polyval: evaluates polynomial at x
        y_pred = np.polyval(coeffs, x)
        
        return {
            'success': True,
            'predictions': y_pred.tolist()
        }
    except Exception as e:
        return {'success': False, 'error': str(e)}
